using Amazon;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.Model;
using Amazon.Runtime;
using Amazon.Runtime.CredentialManagement;

namespace Api;

public class ProfileRepository : IProfileRepository
{
    private AmazonDynamoDBClient _client;

    public ProfileRepository()
    {
        var chain = new CredentialProfileStoreChain();
        AWSCredentials credentials;
        _client = chain.TryGetAWSCredentials("profile1", out credentials)
            ? new AmazonDynamoDBClient(credentials, RegionEndpoint.USEast1)
            : new AmazonDynamoDBClient(new AmazonDynamoDBConfig {RegionEndpoint = RegionEndpoint.USEast1});
    }

    public async Task<Profile> GetProfile(string customerId)
    {
        var record = await _client.GetItemAsync("profile",
            new Dictionary<string, AttributeValue> {{"customer_id", new AttributeValue(customerId)}});

        var fields = record.Item;
       // string customerId, string firstName, string lastName, string phoneNumber, string email
        fields.TryGetValue("first_name", out var firstName);
        fields.TryGetValue("last_name", out var lastName);
        fields.TryGetValue("email", out var email);
        fields.TryGetValue("phone_number", out var phoneNumber);

        var profile = new Profile(customerId, firstName?.S??string.Empty,
            lastName?.S??string.Empty, phoneNumber?.S??string.Empty, 
            email?.S??string.Empty);
   
        return profile;
    }
}