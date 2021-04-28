namespace KafkaFlow.Sample.Dashboard
{
    using KafkaFlow.Admin.Dashboard;
    using Microsoft.AspNetCore.Builder;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Hosting;
    
    public class Startup
    {
        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddKafka(
                kafka => kafka
                    .AddCluster(
                        cluster => cluster
                            .WithBrokers(new[] { "localhost:9092" })
                            .EnableAdminMessages("kafka-flow.admin", "kafka-flow.admin.group.id")
                            .AddConsumer(
                                consumer => consumer
                                    .Topic("topic-dashboard")
                                    .WithGroupId("groupid-dashboard")
                                    .WithName("consumer-dashboard")
                                    .WithBufferSize(100)
                                    .WithWorkersCount(20)
                                    .WithAutoOffsetReset(AutoOffsetReset.Latest)
                            ))
            );

            services.AddControllers();

            services.AddKafkaFlowDashboard();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, IHostApplicationLifetime lifetime)
        {
            app.UseRouting();
            
            app.UseEndpoints(endpoints => { endpoints.MapControllers(); });

            app.UseKafkaFlowDashboard(env);

            lifetime.ApplicationStarted.Register(() => app.ApplicationServices.CreateKafkaBus().StartAsync(lifetime.ApplicationStopped));
        }
    }
}