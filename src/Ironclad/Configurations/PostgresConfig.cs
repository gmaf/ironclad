namespace Ironclad.Configurations
{
    public sealed class PostgresConfig
    {
        public string Host { get; set; }

        public string Port { get; set; }

        public string Database { get; set; }

        public string UserId { get; set; }

        public string Password { get; set; }

        public override string ToString()
            => $"host={this.Host};port={this.Port};userid={this.UserId};password={this.Password};database={this.Database}";
    }
}
