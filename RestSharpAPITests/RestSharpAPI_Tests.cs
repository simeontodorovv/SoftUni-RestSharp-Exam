using RestSharp;
using RestSharpAPITests.Models;
using System.Net;
using System.Text.Json;

namespace RestSharpAPITests
{
    public class RestSharpAPI_Tests
    {
        private RestClient restClient;
        private const string baseUrl = "https://contactbook.softuniqa.repl.co/";

        [SetUp]
        public void Setup()
        {
             restClient = new RestClient(baseUrl);
        }

        [Test]
        public void FirstContactShouldContainsSteveJobsContact()
        {
            var expectedFirstName = "Steve";
            var expectedLastName = "Jobs";
            RestRequest request = new RestRequest("/api/contacts");

            var response = restClient.Execute(request);

            var contacts = JsonSerializer.Deserialize<List<ContactDTO>>(response.Content);

            var firstContact = contacts.FirstOrDefault();

            Assert.IsNotNull(firstContact);
            Assert.That(firstContact.FirstName, Is.EqualTo(expectedFirstName));
            Assert.That(firstContact.LastName, Is.EqualTo(expectedLastName));
        }

        [Test]
        public void FirstContactByKeywordShouldBeAlbertEinstein()
        {
            var expectedFirstName = "Albert";
            var expectedLastName = "Einstein";
            RestRequest request = new RestRequest("/api/contacts/search/albert");

            var response = restClient.Execute(request);

            var contact = JsonSerializer.Deserialize<List<ContactDTO>>(response.Content).FirstOrDefault(x => x.FirstName == expectedFirstName && x.LastName == expectedLastName);

            Assert.IsNotNull(contact);
            Assert.That(contact.FirstName, Is.EqualTo(expectedFirstName));
            Assert.That(contact.LastName, Is.EqualTo(expectedLastName));
        }

        [Test]
        public void InvalidContactSearchWordShouldReturnEmptyContact()
        {
            RestRequest request = new RestRequest("/api/contacts/search/missing{random_number}");

            var response = restClient.Execute(request);

            var contacts = JsonSerializer.Deserialize<List<ContactDTO>>(response.Content);

            Assert.IsNotNull(contacts);
            CollectionAssert.IsEmpty(contacts);
        }

        [Test]
        public void CreateContactWithInvalidDataShouldReturnError()
        {
            var expectedMsg = "First name cannot be empty!";
            RestRequest request = new RestRequest("/api/contacts", Method.Post);

            var body = new { };
            request.AddBody(body);
            var response = restClient.Execute(request);

            var err = JsonSerializer.Deserialize<ErrorDTO>(response.Content);

            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));
            Assert.That(err.Message, Is.EqualTo(expectedMsg));
        }

        [Test]
        public void CreateContactShouldCreateNewContact()
        {
            var expectedMsg = "Contact added.";
            RestRequest request = new RestRequest("/api/contacts", Method.Post);

            var body = new { firstName = "Marie", lastName = "Curie", email = "marie67@gmail.com", phone = "+1 800 200 300", comments = "Old friend"};
            request.AddBody(body);
            var response = restClient.Execute(request);

            var createdContactResponse = JsonSerializer.Deserialize<CreateContactDTO>(response.Content);

            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.Created));
            Assert.That(createdContactResponse, Is.Not.Null);
            Assert.That(createdContactResponse.Message, Is.EqualTo(expectedMsg));
            Assert.That(createdContactResponse.Contact, Is.Not.Null);
            Assert.That(createdContactResponse.Contact.FirstName, Is.EqualTo(body.firstName));
            Assert.That(createdContactResponse.Contact.LastName, Is.EqualTo(body.lastName));
            Assert.That(createdContactResponse.Contact.Email, Is.EqualTo(body.email));
            Assert.That(createdContactResponse.Contact.Phone, Is.EqualTo(body.phone));
            Assert.That(createdContactResponse.Contact.Comments, Is.EqualTo(body.comments));
        }
    }
}