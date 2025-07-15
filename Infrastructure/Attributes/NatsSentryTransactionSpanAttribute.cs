using PostSharp.Aspects;
using PostSharp.Serialization;


namespace OrderService.Infrastructure.Attributes
{
    [PSerializable]
    public class NatsSentryTransactionSpanAttribute : OnMethodBoundaryAspect
    {
        private ISpan Span;

        private static (string, string, string, string, string) GetMetaFromArgs(MethodExecutionArgs args) {
            // Create a span name
            var className = args.Method.DeclaringType.FullName;
            var methodName = args.Method.Name;
            var subject = args.Arguments.ElementAt(0).ToString();
            var data = args.Arguments.ElementAt(1).ToString();

            string spanName = $"{className}.{methodName}";

            return (
                className,
                methodName,
                subject,
                data,
                spanName
            );
        }
        
        private static ISpan GetSpan(MethodExecutionArgs args) {
            var (
                _,
                _,
                subject,
                _,
                spanName
            ) = GetMetaFromArgs(args);
        
            // Get current span or create transaction
            var parentSpan = SentrySdk.GetSpan();
            ISpan span;

            if (parentSpan != null)
            {
                span = parentSpan.StartChild(spanName.ToLower(), subject);
            }
            else
            {
                span = SentrySdk.StartTransaction(
                    spanName.ToLower(),
                    subject
                );
            }

            return span;
        }
        
        public override void OnEntry(MethodExecutionArgs args)
        {
            var (
                className,
                methodName,
                subject,
                data,
                _
            ) = GetMetaFromArgs(args);

            Span = GetSpan(args);
            
            // Add relevant information to the span
            Span.SetTag("nats.subject", subject);
            Span.SetTag("class.method", $"{className}.{methodName}");

            // Add data information if available (carefully to avoid sensitive data)
            if (args.Arguments.Count > 1 && args.Arguments[1] != null)
            {
                Span.SetTag("data.type", data.GetType().Name);
            }
        }

        public override void OnSuccess(MethodExecutionArgs args)
        {
            Span.Status = SpanStatus.Ok;
            Span.Finish();
        }

        public override void OnException(MethodExecutionArgs args)
        {
            Span.Status = SpanStatus.InternalError;
            SentrySdk.CaptureException(args.Exception);
            Span.Finish();
        }
    }
}