using OrderService.Commands;
using RuangDeveloper.AspNetCore.Command;

namespace OrderService
{
    public partial class Startup
    {
        public void Commands(IServiceCollection services)
        {
            services.AddCommands(configure => {
                configure.AddCommand<SeederCommand>();
            });
        }
    }
}
