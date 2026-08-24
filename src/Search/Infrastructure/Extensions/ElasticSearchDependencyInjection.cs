namespace Search.Infrastructure.Extensions;

public static class ElasticSearchDependencyInjection
{
    public static void ElasticSearchConfigure(this IHostApplicationBuilder builder)
    {

        builder.Services.AddSingleton(sp =>
        {
            var elasticSettings = sp.GetRequiredService<IOptions<AppSettings>>().Value.ElasticSearchOptions;

            var settings = new ElasticsearchClientSettings(new Uri(elasticSettings.Host))
                                        .CertificateFingerprint(elasticSettings.Fingerprint)
                                        .Authentication(new BasicAuthentication(elasticSettings.UserName, elasticSettings.Password));

            return new ElasticsearchClient(settings);
        });


        //System.Diagnostics.Debug.WriteLine(">>> ElasticSearchConfigure CALLED");

        //builder.Services.AddSingleton(sp =>
        //{
        //    var elasticSettings = sp
        //        .GetRequiredService<IOptions<AppSettings>>()
        //        .Value
        //        .ElasticSearchOptions;

        //    System.Diagnostics.Debug.WriteLine(
        //        $">>> Elastic Host: {elasticSettings.Host}");

        //    System.Diagnostics.Debug.WriteLine(
        //        $">>> Elastic Fingerprint: {elasticSettings.Fingerprint}");

        //    var settings = new ElasticsearchClientSettings(
        //            new Uri(elasticSettings.Host))
        //        .CertificateFingerprint(elasticSettings.Fingerprint.Trim())
        //        .Authentication(
        //            new BasicAuthentication(
        //                elasticSettings.UserName,
        //                elasticSettings.Password));

        //    return new ElasticsearchClient(settings);
        //});

    }
}
