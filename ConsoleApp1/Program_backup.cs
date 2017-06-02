using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using Newtonsoft.Json;

using DocuSign.eSign.Api;
using DocuSign.eSign.Model;
using DocuSign.eSign.Client;
using System.Net;

namespace TestProj
{
    class CoreRecipes
    {
        // Integrator Key (aka API key) is needed to authenticate your API calls.  This is an application-wide key
        private string INTEGRATOR_KEY = "a2dced5d-bebe-4b70-803e-bbbb70ecf7cb";

        //////////////////////////////////////////////////////////
        // Main()
        //////////////////////////////////////////////////////////
        static void Main(string[] args)
        {
            CoreRecipes recipes = new CoreRecipes();

            EnvelopeSummary envSummary = recipes.requestSignatureFromTemplateTest();

            Console.Read();
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public EnvelopeSummary requestSignatureFromTemplateTest()
        {
            // Enter your DocuSign credentials below.  Note: You only need a DocuSign account to SEND documents,
            // signing is always free and signers do not need an account.
            string username = "epercic@ripcordtech.ca";
            string password = "epercic";

            // Enter the contract data
            string rent_amount_input = "$550";
            string property_address_input = "123 Lakedrive Rd.";

            // Enter the signature area layout
            int SIGNATURE_COLUMNS = 3;

            // the document (file) we want signed
            string templateId = "37c5956a-2fab-447e-995c-fb3733fb866d";


            // (name, email, adult, primary_signer
            //List<Tuple<string, string, bool, bool>> listOfPeople = new List<Tuple<string, string, bool, bool>> {
            //    Tuple.Create("Erik Percic", "3r1k0n@gmail.com", true, true),
            //    Tuple.Create("Kim Sabo", "erik.percic@gmail.com", true, false),
            //    Tuple.Create("Marco Marchi", "erik.percic@gmail.com", true, false),
            //    Tuple.Create("Dotcom Harvey", "erik.percic@gmail.com", true, false),
            //    Tuple.Create("Senor Fuego", "erik.percic@gmail.com", true, false),
            //    Tuple.Create("Suleima Diaz", "erik.percic0708@gmail.com", true, false),
            //    Tuple.Create("Jackie Salas", "erik.percic@hotmail.com", false, false)
            // };

            List<Person> listOfPeople = new List<Person> {
                new Person { Name="Erik Percic", Email = "3r1k0n@gmail.com", Adult=true, PrimarySigner=true},

                new Person { Name="Kim Sabo", Email = "erik.percic@gmail.com", Adult=true},
                new Person { Name="Marco Marchi", Email = "erik.percic@gmail.com", Adult=true},
                new Person { Name="Senor Fuego", Email = "erik.percic@gmail.com", Adult=true},
                new Person { Name="Dotcom Harvey", Email = "erik.percic@gmail.com", Adult=true},

                new Person { Name="Suleima Diaz", Email = "erik.percic0708@gmail.com", Adult=true},

                new Person { Name="Jackie Salas", Email = "erik.percic@hotmail.com", Adult=false},
             };

            // instantiate api client with appropriate environment (for production change to www.docusign.net/restapi)
            configureApiClient("https://demo.docusign.net/restapi");

            //===========================================================
            // Step 1: Login()
            //===========================================================

            // call the Login() API which sets the user's baseUrl and returns their accountId
            string accountId = loginApi(username, password);

            //===========================================================
            // Step 2: Signature Request from Template 
            //===========================================================

            EnvelopeDefinition envDef = new EnvelopeDefinition();
            envDef.EmailSubject = "[DocuSign C# SDK] - Please sign this doc - Template 2";

            // assign recipient to template role by setting name, email, and role name.  Note that the
            // template role name must match the placeholder role name saved in your account template.

            List<TemplateRole> listOfRoles = new List<TemplateRole>();
            Person[] arrayOfAdults = listOfPeople.Where(x => x.Adult == true).ToArray();
            for (int i=0;i<arrayOfAdults.Length;i++)
            {
                Person p = arrayOfAdults[i];

                TemplateRole tRole = new TemplateRole();
                tRole.Tabs = new Tabs();
                tRole.Tabs.TextTabs = new List<Text>();
                tRole.Tabs.SignHereTabs = new List<SignHere>();

                tRole.Name = p.Name;
                tRole.Email = p.Email;
                
                if (p.PrimarySigner) //person is primary signer
                {
                    tRole.RoleName = "Primary Signer";

                    Text rent_amount_tab = new Text();
                    rent_amount_tab.TabLabel = "\\*rent_amount";
                    rent_amount_tab.Value = rent_amount_input;

                    Text property_address_tab = new Text();
                    property_address_tab.TabLabel = "\\*property_address";
                    property_address_tab.Value = property_address_input;

                    Text other_occupants_tab = new Text();
                    other_occupants_tab.TabLabel = "\\*other_occupants";
                    other_occupants_tab.Value = String.Join(", ", arrayOfAdults.Select(x => x.Name).ToArray());

                    for(int j = 0; j < arrayOfAdults.Length;j++)
                    {
                        Text lineTab = new Text();

                        lineTab.DocumentId = "1";
                        lineTab.AnchorString = "Signatures";
                        lineTab.Name = lineTab.TabLabel = "Line";
                        lineTab.AnchorCaseSensitive = "true";
                        lineTab.AnchorXOffset = ((j%SIGNATURE_COLUMNS) * 100).ToString();
                        lineTab.AnchorYOffset = ((j/3)*80+60).ToString();
                        lineTab.AnchorUnits = "pixels";
                        lineTab.PageNumber = "1";

                        lineTab.Value = "_______________";
                        lineTab.Locked = "true";
                        lineTab.Required = "false";

                        tRole.Tabs.TextTabs.Add(lineTab);

                        Text nameTab = new Text();

                        nameTab.DocumentId = "1";
                        nameTab.AnchorString = "Signatures";
                        nameTab.Name = nameTab.TabLabel = "Name";
                        nameTab.AnchorCaseSensitive = "true";
                        nameTab.AnchorXOffset = ((j % SIGNATURE_COLUMNS) * 100+3).ToString();
                        nameTab.AnchorYOffset = ((j / 3) * 80 + 60+10).ToString();
                        nameTab.AnchorUnits = "pixels";
                        nameTab.PageNumber = "1";

                        nameTab.Value = arrayOfAdults[j].Name;
                        nameTab.Locked = "true";
                        nameTab.Required = "false";

                        tRole.Tabs.TextTabs.Add(nameTab);
                    }

                    tRole.Tabs.TextTabs.AddRange(new List<Text> { rent_amount_tab, property_address_tab, other_occupants_tab });
                }
                else
                {
                    tRole.RoleName = "Adult " + (i + 2).ToString();
                }

                //calculate signature offsets
                int signatureOffsetY = ((i / 3) * 80) + 60;
                int signatureOffsetX = ((i % SIGNATURE_COLUMNS) * 100);

                SignHere signatureTab = new SignHere();

                signatureTab.DocumentId =  "1";
                signatureTab.AnchorString = "Signatures";
                signatureTab.Name = signatureTab.TabLabel = "Sign Here";
                signatureTab.AnchorCaseSensitive = "true";
                signatureOffsetX.ToString();
                signatureTab.AnchorXOffset = (signatureOffsetX + 20).ToString();
                signatureTab.AnchorYOffset = signatureOffsetY.ToString();
                signatureTab.AnchorUnits = "pixels";
                signatureTab.PageNumber = "1";

                tRole.Tabs.SignHereTabs.Add(signatureTab);
                listOfRoles.Add(tRole);
            }

            // add the role to the envelope and assign valid templateId from your account
            envDef.TemplateRoles = listOfRoles;
            envDef.TemplateId = templateId;

            // set envelope status to "sent" to immediately send the signature request
            envDef.Status = "sent";

            // |EnvelopesApi| contains methods related to creating and sending Envelopes (aka signature requests)
            EnvelopesApi envelopesApi = new EnvelopesApi();
            EnvelopeSummary envelopeSummary = envelopesApi.CreateEnvelope(accountId, envDef);

            // print the JSON response
            Console.WriteLine("EnvelopeSummary:\n{0}", JsonConvert.SerializeObject(envelopeSummary));

            return envelopeSummary;

        } // end requestSignatureFromTemplateTest()


        //**********************************************************************************************************************
        //**********************************************************************************************************************
        //*  HELPER FUNCTIONS
        //**********************************************************************************************************************
        //**********************************************************************************************************************
        public void configureApiClient(string basePath)
        {
            // instantiate a new api client
            ApiClient apiClient = new ApiClient(basePath);

            // set client in global config so we don't need to pass it to each API object.
            Configuration.Default.ApiClient = apiClient;
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public string loginApi(string usr, string pwd)
        {
            // we set the api client in global config when we configured the client 
            ApiClient apiClient = Configuration.Default.ApiClient;
            string authHeader = "{\"Username\":\"" + usr + "\", \"Password\":\"" + pwd + "\", \"IntegratorKey\":\"" + INTEGRATOR_KEY + "\"}";
            Configuration.Default.AddDefaultHeader("X-DocuSign-Authentication", authHeader);

            // we will retrieve this from the login() results
            string accountId = null;

            // the authentication api uses the apiClient (and X-DocuSign-Authentication header) that are set in Configuration object
            AuthenticationApi authApi = new AuthenticationApi();
            LoginInformation loginInfo = authApi.Login();

            // find the default account for this user
            foreach (LoginAccount loginAcct in loginInfo.LoginAccounts)
            {
                if (loginAcct.IsDefault == "true")
                {
                    accountId = loginAcct.AccountId;
                    break;
                }
            }
            if (accountId == null)
            { // if no default found set to first account
                accountId = loginInfo.LoginAccounts[0].AccountId;
            }
            return accountId;
        }

    } // end class
    public class Person
    {
        public string Name { get; set; }
        public string Email { get; set; }
        public bool Adult { get; set; }
        public bool PrimarySigner { get; set; } = false;
    }
} // end namespace
