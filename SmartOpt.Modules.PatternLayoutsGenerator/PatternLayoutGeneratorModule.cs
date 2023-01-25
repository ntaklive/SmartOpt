using Ninject.Syntax;
using SmartOpt.Modules.PatternLayoutsGenerator.Services;
using SmartOpt.Modules.PatternLayoutsGenerator.Services.Abstractions;

namespace SmartOpt.Modules.PatternLayoutsGenerator
{
    public class PatternLayoutGeneratorModule : ModuleBase
    {
        public override void ConfigureServices(IBindingRoot services)
        {
            services.Bind<IOrderInfoParser>().To<OrderInfoParser>().InSingletonScope();
            services.Bind<IPatternLayoutGenerator>().To<PatternLayoutGenerator>().InSingletonScope();
            services.Bind<IPatternLayoutService>().To<PatternLayoutService>().InSingletonScope();
            services.Bind<IReportExporter>().To<ReportExporter>().InSingletonScope();
        }
    }
}