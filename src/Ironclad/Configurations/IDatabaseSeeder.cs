namespace Ironclad.Configurations
{
    using Microsoft.AspNetCore.Builder;

    public interface IDatabaseSeeder
    {
        void Seed(IApplicationBuilder app);
    }
}
