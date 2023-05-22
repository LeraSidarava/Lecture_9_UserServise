using System.Net;
using System.Text;
using Newtonsoft.Json;
using UserService.Models.Responses;
using NUnit.Framework;
using System.Net.Http;
using CatalogTest.UserStatus;
using static System.Net.WebRequestMethods;
using System.Linq;
using UserService.Clients;
using System.Net.Mail;


namespace UserService;

public class CatalogTests
{
    private string randomString;

    [SetUp]

    public void Setup()
    {
        randomString = GenerateRandomString(10);
    }

    private string GenerateRandomString(int length)
    {
        const string chars = "abcdefghijklmnopqrstuvwxyz";
        var random = new Random();

        return new string(Enumerable.Repeat(chars, length)
            .Select(c => c[random.Next(c.Length)]).ToArray());
    }


   




    [Test]
    public async Task Test_1_RegicterUser_EmptyFields_ResponseCodeIsOK()
    {
        //Preconditions


        CatalogServiceClient client = new CatalogServiceClient();

        CreateUserRequest requestBody = new CreateUserRequest()//new instance of body by using initializer
        {
            FirstName = "",
            LastName = "",
        };

        int userId = await client.CreateUserRequest(requestBody);
        Assert.That(userId, Is.GreaterThan(16000));




    }

    [Test]
    public async Task Test_2_Rejicterser_NullFields_ResponseIsNewUserID_CodeIsOK()
    {
        //Preconditions
        CatalogServiceClient client = new CatalogServiceClient();

        CreateUserRequest requestBody = new CreateUserRequest()//new instance of body by using initializer
        {
            FirstName = null,
            LastName = null,
        };

        //Actions

        int userId = await client.CreateUserRequest(requestBody);

        //Asssert

        HttpStatusCode statusCode = await client.GetUserStatusCode(userId);

        Assert.That(statusCode, Is.EqualTo(HttpStatusCode.OK));


    }



    [Test]
    public async Task Test_3_RegisterUser_FieldsInputDigits_StatusCodeIsOK()
    {
        // Preconditions
        CatalogServiceClient client = new CatalogServiceClient();

        string firstName = RandomString3(10);
        string lastName = RandomString3(10);

        CreateUserRequest requestBody = new CreateUserRequest()
        {
            FirstName = firstName,
            LastName = lastName
        };

        // Actions
        int userId = await client.CreateUserRequest(requestBody);



        // Assert
        HttpStatusCode statusCode = await client.GetUserStatusCode(userId);
        Assert.That(statusCode, Is.EqualTo(HttpStatusCode.OK));
    }

    private string RandomString3(int length)
    {
        const string chars = "0123456789";
        var random = new Random();

        return new string(Enumerable.Repeat(chars, length)
            .Select(c => c[random.Next(c.Length)]).ToArray());
    }



    [Test]
    public async Task Test_4_RegisterUser_FieldsInputSpecialChars_StatusCodeIsOK()
    {
        // Preconditions
        CatalogServiceClient client = new CatalogServiceClient();

        string firstName = RandomString4(10);
        string lastName = RandomString4(10);

        CreateUserRequest requestBody = new CreateUserRequest()
        {
            FirstName = firstName,
            LastName = lastName
        };

        // Actions
        int userId = await client.CreateUserRequest(requestBody);

        // Assert
        Assert.That(userId, Is.GreaterThan(0));

        HttpStatusCode statusCode = await client.GetUserStatusCode(userId);
        Assert.That(statusCode, Is.EqualTo(HttpStatusCode.OK));
    }
    private string RandomString4(int length)
    {
        const string chars = "!@#$%^&*()_+{}|:{}[]`~/.,<>?";
        var random = new Random();

        return new string(Enumerable.Repeat(chars, length)
            .Select(c => c[random.Next(c.Length)]).ToArray());
    }



   


    [Test]
    public async Task Test_6_RegisterUser_FieldsInput101Symbols_StatusCodeIsOK()
    {
        // Preconditions
        CatalogServiceClient client = new CatalogServiceClient();

        string firstName = GenerateRandomString(101).ToUpper();
        string lastName = GenerateRandomString(101).ToUpper();

        CreateUserRequest request = new CreateUserRequest()
        {
            FirstName = firstName,
            LastName = lastName
        };

        // Actions
        int userId = await client.CreateUserRequest(request);

        // Assert
        Assert.That(userId, Is.GreaterThan(0));

        HttpStatusCode statusCode = await client.GetUserStatusCode(userId);
        Assert.That(statusCode, Is.EqualTo(HttpStatusCode.OK));
    }

   

    [Test]
    public async Task Test_7_RegisterUser_FieldsInputUpperCase_StatusCodeIsOK()
    {
        // Preconditions
        CatalogServiceClient client = new CatalogServiceClient();

        string firstName = GenerateRandomString(10).ToUpper();
        string lastName = GenerateRandomString(10).ToUpper();

        CreateUserRequest request = new CreateUserRequest()
        {
            FirstName = firstName,
            LastName = lastName,
        };

        // Actions
        int userId = await client.CreateUserRequest(request);

        // Assert
        Assert.That(userId, Is.GreaterThan(0));

        HttpStatusCode statusCode = await client.GetUserStatusCode(userId);
        Assert.That(statusCode, Is.EqualTo(HttpStatusCode.OK));
    }

    [Test]
    public async Task Test_8_CreateMultipleUsers_AutoIncrementedIds()
    {
        // Preconditions
        CatalogServiceClient client = new CatalogServiceClient();
        int previousUserId = 0;

        for (int i = 0; i < 2; i++) // Create 2 users
        {
            string firstName = GenerateRandomString(10);
            string lastName = GenerateRandomString(10);

            CreateUserRequest requestbody = new CreateUserRequest()
            {
                FirstName = firstName,
                LastName = lastName
            };

            // Send the create user request
            int userId = await client.CreateUserRequest(requestbody);

            // Assert
            Assert.That(userId, Is.GreaterThan(0)); // Verify that the user ID is greater than 0

            // Verify auto-incremented IDs
            if (i > 0)
            {
                // Assert that the returned user ID is exactly one greater than the previous user's ID
                Assert.That(userId, Is.EqualTo(previousUserId + 1));
            }

            previousUserId = userId; // Store the current user ID as the previous user's ID for the next iteration
        }
    }


    [Test]
    public async Task Test_9_CreateUser1_DeleteUser1_CreateUser2_User2Id_Is_User1IdPlus1()
    {
        // Preconditions
        CatalogServiceClient client = new CatalogServiceClient();
        int previousUserId = 0;

        // Create the first user
        string firstName = GenerateRandomString(10);
        string lastName = GenerateRandomString(10);

        CreateUserRequest requestBody = new CreateUserRequest()
        {
            FirstName = firstName,
            LastName = lastName
        };

        // Send the create user request
        int userId = await client.CreateUserRequest(requestBody);

        // Assert
        Assert.That(userId, Is.GreaterThan(16000)); // Verify that the user ID is greater than 0

        previousUserId = userId; // Store the current user ID as the previous user's ID

        // Delete the first user
        HttpStatusCode deleteStatusCode = await client.DeleteUser(previousUserId);
        Assert.That(deleteStatusCode, Is.EqualTo(HttpStatusCode.OK));

        // Create the second user
        firstName = GenerateRandomString(10);
        lastName = GenerateRandomString(10);

        requestBody = new CreateUserRequest()
        {
            FirstName = firstName,
            LastName = lastName
        };

        // Send the create user request
        userId = await client.CreateUserRequest(requestBody);

        // Assert
        Assert.That(userId, Is.EqualTo(previousUserId + 1)); // Verify that the user ID is one greater than the previous user's ID
    }

    [Test]
    public async Task Test_10_RegisterUser__DeleteNonactiveUser_GetUserStatus_StatusCodeIs404()
    {
        // Preconditions
        CatalogServiceClient client = new CatalogServiceClient();

        string firstName = GenerateRandomString(10);
        string lastName = GenerateRandomString(10);

        CreateUserRequest requestbody = new CreateUserRequest()
        {
            FirstName = firstName,
            LastName = lastName
        };



        // Actions
        int userId = await client.CreateUserRequest(requestbody);
        bool userStatus = await client.GetUserStatus(userId);

        // Assert
        Assert.That(userStatus, Is.False);

        // Delete nonactive user
        HttpStatusCode deleteResult = await client.DeleteUser(userId);

        Assert.That(deleteResult, Is.EqualTo(HttpStatusCode.OK));

        //Get deleted ser status
        HttpStatusCode getUserStatusStatusCode = await client.GetUserStatusCode(userId);


        // Assert
        Assert.That(getUserStatusStatusCode, Is.EqualTo(HttpStatusCode.NotFound));



    }


    [Test]
    public async Task Test_11_RegisterUser_GetUserId_CheckUserStatus_StatusIsFalse()
    {
        // Preconditions
        CatalogServiceClient client = new CatalogServiceClient();

        string firstName = GenerateRandomString(10);
        string lastName = GenerateRandomString(10);

        CreateUserRequest requestbody = new CreateUserRequest()
        {
            FirstName = firstName,
            LastName = lastName
        };

        // Send the create user request
        int userId = await client.CreateUserRequest(requestbody);

        Assert.That(userId, Is.GreaterThan(0));


        // Check user status by userId
        bool userStatus = await client.GetUserStatus(userId);
        // Assert
        Assert.That(userStatus, Is.False);


    }

    



    [Test]
    public async Task Test_12and15_RegisterUser_GetUserId_ChangeUserStatusToTrue_StatusIsTrue()
    {
        // Preconditions
        CatalogServiceClient client = new CatalogServiceClient();

        string firstName = GenerateRandomString(10);
        string lastName = GenerateRandomString(10);

        CreateUserRequest requestbody = new CreateUserRequest()
        {
            FirstName = firstName,
            LastName = lastName
        };

        int userId = await client.CreateUserRequest(requestbody);

        Assert.That(userId, Is.GreaterThan(0));

        // Change user status from false to true
        bool setUserStatusResult = await client.SetUserStatus(userId, true);

        // Assert
        Assert.That(setUserStatusResult, Is.True);

        // Check user status by userId
        bool userStatus = await client.GetUserStatus(userId);

        // Assert
        Assert.That(userStatus, Is.True);
    }

    [Test]
    public async Task Test_13and16_RegisterUser_GetUserId_ChangeUserStatusTrueToFalse_StatusIsFalse()
    {
        // Preconditions
        CatalogServiceClient client = new CatalogServiceClient();

        string firstName = GenerateRandomString(10);
        string lastName = GenerateRandomString(10);



        CreateUserRequest requestbody = new CreateUserRequest()
        {
            FirstName = firstName,
            LastName = lastName
        };



        // Actions 
        int userId = await client.CreateUserRequest(requestbody);

        // Change user status from true to false
        bool setResultTrue = await client.SetUserStatus(userId, true);
        bool setResultFalse = await client.SetUserStatus(userId, false);

        // Assert
        Assert.That(setResultTrue, Is.True);
        Assert.That(setResultFalse, Is.True);

        // Check user status by userId
        bool userStatus = await client.GetUserStatus(userId);

        // Assert
        Assert.That(userStatus, Is.False);


    }

    [Test]
    public async Task Test_14_RegisterUser__DeleteNonactiveUser_SetUserStatusTrue_StatusCodeIs404()
    {
        // Preconditions
        CatalogServiceClient client = new CatalogServiceClient();

        string firstName = GenerateRandomString(10);
        string lastName = GenerateRandomString(10);

        CreateUserRequest requestbody = new CreateUserRequest()
        {
            FirstName = firstName,
            LastName = lastName
        };



        // Actions
        int userId = await client.CreateUserRequest(requestbody);
        bool userStatus = await client.GetUserStatus(userId);

        // Assert
        Assert.That(userStatus, Is.False);

        // Delete nonactive user
        HttpStatusCode deleteResult = await client.DeleteUser(userId);
        Assert.That(deleteResult, Is.EqualTo(HttpStatusCode.OK));

        // 
        bool setUserStatusResult = await client.SetUserStatus(userId, true);

        // Assert

        HttpStatusCode getUserStatusResult = await client.GetUserStatusCode(userId);
        Assert.That(getUserStatusResult, Is.EqualTo(HttpStatusCode.NotFound));
    }

    [Test]
    public async Task Test_20_RegisterUser_GetUserId_CheckUserStatus_StatusIsFalse_DeleteNonactiveUser_StatusCodeIs200()
    {
        // Preconditions
        CatalogServiceClient client = new CatalogServiceClient();

        string firstName = GenerateRandomString(10);
        string lastName = GenerateRandomString(10);

        CreateUserRequest requestbody = new CreateUserRequest()
        {
            FirstName = firstName,
            LastName = lastName
        };

        int userId = await client.CreateUserRequest(requestbody);



        // Check user status by userId
        bool userStatus = await client.GetUserStatus(userId);



        //Assert

        Assert.That(userStatus, Is.False);

        //Delete nonactive user

        HttpStatusCode deleteNonExistingStatusCode = await client.DeleteUser(userId);
        Assert.That(deleteNonExistingStatusCode, Is.EqualTo(HttpStatusCode.OK));


    }



    [Test]
    public async Task Test_21_RegisterUser_DeleteNonactiveUser_DeleteNonExistingUser_StatusCodeIs500()
    {
        // Preconditions
        CatalogServiceClient client = new CatalogServiceClient();

        string firstName = GenerateRandomString(10);
        string lastName = GenerateRandomString(10);

        CreateUserRequest requestbody = new CreateUserRequest()
        {
            FirstName = firstName,
            LastName = lastName
        };

        // Actions
        int userId = await client.CreateUserRequest(requestbody);

        // Check user status by userId
        bool userStatus = await client.GetUserStatus(userId);

        // Assert
        Assert.That(userStatus, Is.False);

        // Delete nonactive user
        HttpStatusCode deleteResult = await client.DeleteUser(userId);

        Assert.That(deleteResult, Is.EqualTo(HttpStatusCode.OK));

        // Delete non-existing user

        HttpStatusCode deleteNonExistingResult = await client.DeleteUser(userId);

        Assert.That(deleteNonExistingResult, Is.EqualTo(HttpStatusCode.InternalServerError));



    }


































}


