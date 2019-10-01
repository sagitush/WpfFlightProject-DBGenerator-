using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace WpfAppFlightProject
{
    public class DBGenerator
    {
        public int Total { get; set; }
        private const string URL1 = "https://randomuser.me/api";
        private const string URL2 = "https://restcountries.eu/rest/v2";
        static Random rnd = new Random();
        public double Percent { get; set; }
        public string Message { get; set; }

        /// <summary>
        /// Add random customers to the database.
        /// </summary>
        /// <param name="customers"></param>
        public void AddCustomersToDB(int customers)
        {
            for (int i = 0; i < customers; i++)
            {

                HttpClient client = new HttpClient();

                client.BaseAddress = new Uri(URL1);

                client.DefaultRequestHeaders.Accept.Add(
                    new MediaTypeWithQualityHeaderValue("application/json"));

                HttpResponseMessage response = client.GetAsync("").Result;

                if (response.IsSuccessStatusCode)
                {
                    FlightCenterProject.CustomerDAOMSSQL customerDAOMSSQL = new FlightCenterProject.CustomerDAOMSSQL();

                    WebResult dataObjects = response.Content.ReadAsAsync<WebResult>().Result;

                    string phoneNum = dataObjects.results[0].phone;
                    string creditCard = dataObjects.results[0].cell;//replace the credit card number.
                    string userName = dataObjects.results[0].login.username;
                    string pass = dataObjects.results[0].login.password;
                    string address = dataObjects.results[0].location.city;
                    string firstName = dataObjects.results[0].name.first;
                    string lastName = dataObjects.results[0].name.last;

                    FlightCenterProject.Customer customer = new FlightCenterProject.Customer(firstName, lastName, userName, pass, address, phoneNum, creditCard);

                    customerDAOMSSQL.Add(customer);
                }
                else
                {
                    Debug.Write((int)response.StatusCode, response.ReasonPhrase);
                }

                    client.Dispose();
            
                    Percent += Convert.ToInt32(1.0 / Total * 100);
                
                    Message = $"{i + 1}/{customers} Customers created";

            }
        }

        /// <summary>
        /// Add random countries to the database.
        /// </summary>
        /// <param name="countries"></param>
        public void AddCountriesToDB(int countries)
        {
            
            HttpClient client = new HttpClient();

            client.BaseAddress = new Uri(URL2);

            client.DefaultRequestHeaders.Accept.Add(
                new MediaTypeWithQualityHeaderValue("application/json"));

            HttpResponseMessage response = client.GetAsync("").Result;

            if (response.IsSuccessStatusCode)
            {
                var dataObjects = response.Content.ReadAsAsync<IEnumerable<Country>>().Result;
                int counter = 1;
                foreach (var d in dataObjects)
                {

                    FlightCenterProject.CountryDAOMSSQL countryDAOMSSQL = new FlightCenterProject.CountryDAOMSSQL();
                    string country = d.name;
                    FlightCenterProject.Country c = new FlightCenterProject.Country(country);
                    countryDAOMSSQL.Add(c);
                    if (Total == 0)
                    {
                        Percent = 100;
                        counter = 0;
                    }
                    else
                    {
                        Percent += Convert.ToInt32(1.0 / Total * 100);
                    }
                    Message = $"{counter}/{countries} Countries created";

                    if (++counter > countries)
                        break;
                }
            }
            else
            {
                Debug.Write((int)response.StatusCode, response.ReasonPhrase);
            }

            client.Dispose();
        }

        /// <summary>
        /// Add airline companies from a list of companies randomaly to the database.
        /// </summary>
        /// <param name="airlines"></param>
        /// <param name="airlinesCom"></param>

        public void AddAirlinesToDB(List<string> airlines, int airlinesCom)
        {
            try
            {
                if (airlinesCom > 100)
                {
                    throw new ThereAreNoMoreThan100AirlinesException($"you can not get {airlinesCom} airline companies because the information store has names of 100 airlines");
                }
                else
                {

                    List<string> newList = new List<string>();

                    for (int i = 0; i < airlinesCom; i++)
                    {
                        HttpClient client = new HttpClient();

                        client.BaseAddress = new Uri(URL1);

                        client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                        HttpResponseMessage response = client.GetAsync("").Result;
                        int r = rnd.Next(airlines.Count);
                        int sum = 0;
                        foreach (string company in newList)
                        {
                            if (company == airlines[r])
                            {
                                i = i - 1;
                                break;
                            }
                            else
                            {
                                sum++;
                            }
                        }
                        if (sum == newList.Count)
                        {
                            if (response.IsSuccessStatusCode)
                            {
                                WebResult dataObjects = response.Content.ReadAsAsync<WebResult>().Result;
                                string userName = dataObjects.results[0].login.username;
                                string pass = dataObjects.results[0].login.password;
                                newList.Add(airlines[r]);
                                FlightCenterProject.CountryDAOMSSQL cAOMSSQL = new FlightCenterProject.CountryDAOMSSQL();
                                IList<FlightCenterProject.Country> countries = cAOMSSQL.GetAll();
                                long countryCode = countries[rnd.Next(countries.Count)].Id;
                                FlightCenterProject.AirlineCompanyDAOMSSQL companyDAOMSSQL = new FlightCenterProject.AirlineCompanyDAOMSSQL();
                                FlightCenterProject.AirlineCompany airline = new FlightCenterProject.AirlineCompany(airlines[r], userName, pass, countryCode);
                                companyDAOMSSQL.Add(airline);
                            }
                            else
                            {
                                Debug.Write((int)response.StatusCode, response.ReasonPhrase);
                            }

                            client.Dispose();
                                                      
                            Percent += Convert.ToInt32(1.0 / Total * 100);
                            
                            Message = $"{i + 1}/{airlinesCom} Airline companies created";

                        }
                    }
                }
            }catch (ThereAreNoMoreThan100AirlinesException e)
            {
                Message = e.Message;
            }

        }

        /// <summary>
        /// Add random number of flights for the airline companies that exist in the database.
        /// </summary>
        /// <param name="flights"></param>
        /// <param name="airlinesCom"></param>

        public void AddFlightsToDB(int flights, int airlinesCom)
        {
            FlightCenterProject.AirlineCompanyDAOMSSQL airlineCompany = new FlightCenterProject.AirlineCompanyDAOMSSQL();
            IList<FlightCenterProject.AirlineCompany> airlines = airlineCompany.GetAll();
            try
            {
                if (flights > 0 && airlines.Count == 0)
                {
                    throw new YouNeedToPutNumberOfAirlinesException("You can not get flights if you dont have airlines");
                }
                else
                {
                    for (int i = 0; i < (flights * airlinesCom); i++)
                    {

                        long airlineId = airlines[rnd.Next(airlines.Count)].Id;

                        FlightCenterProject.CountryDAOMSSQL cAOMSSQL = new FlightCenterProject.CountryDAOMSSQL();

                        IList<FlightCenterProject.Country> countries = cAOMSSQL.GetAll();

                        long originCountryCode = countries[rnd.Next(countries.Count)].Id;

                        long destinationCountryCode = countries[rnd.Next(countries.Count)].Id;

                        HttpClient client = new HttpClient();

                        client.BaseAddress = new Uri(URL1);

                        client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                        HttpResponseMessage response = client.GetAsync("").Result;
                        if (response.IsSuccessStatusCode)
                        {
                            WebResult dataObjects = response.Content.ReadAsAsync<WebResult>().Result;

                            DateTime departureTime = dataObjects.results[0].dob.date;

                            DateTime landingTime = dataObjects.results[0].registered.date;

                            DateTime DEPARTURE_TIME = new DateTime(departureTime.Year, departureTime.Month, departureTime.Day, departureTime.Hour, departureTime.Minute, departureTime.Second);
                            DateTime LANDING_TIME = new DateTime(landingTime.Year, landingTime.Month, landingTime.Day, landingTime.Hour, landingTime.Minute, landingTime.Second);

                            FlightCenterProject.FlightDAOMSSQL flightDAOMSSQL = new FlightCenterProject.FlightDAOMSSQL();
                            FlightCenterProject.Flight flight = new FlightCenterProject.Flight(airlineId, originCountryCode, destinationCountryCode, DEPARTURE_TIME, LANDING_TIME, rnd.Next(200));
                            flightDAOMSSQL.Add(flight);

                        }
                        else
                        {
                            Debug.Write((int)response.StatusCode, response.ReasonPhrase);
                        }
                        client.Dispose();
                      
                        Percent += Convert.ToInt32(1.0 / Total * 100);
                        
                        Message = $"{i + 1}/{flights * airlinesCom} Flights created";
                    }
                }
            }catch(YouNeedToPutNumberOfAirlinesException e)
            {
                Message = e.Message;
            }

        }

        /// <summary>
        /// Add random number of tickets for every customer that exist in the database.
        /// </summary>
        /// <param name="ticket"></param>
        /// <param name="customers"></param>
        public void AddTicketsToDB(int ticket, int customers)
        {
            FlightCenterProject.FlightDAOMSSQL flightDAOMSSQL = new FlightCenterProject.FlightDAOMSSQL();
            IList<FlightCenterProject.Flight> flights = flightDAOMSSQL.GetAll();
            FlightCenterProject.CustomerDAOMSSQL customerDAOMSSQL = new FlightCenterProject.CustomerDAOMSSQL();
            IList<FlightCenterProject.Customer> customers2 = customerDAOMSSQL.GetAll();
            try
            { 
            if(ticket>0 && (flights.Count==0 || customers2.Count==0))
            {
                throw new YouNeedToHaveFlightsAndCustomersToGetTicketsException("You can not get tickets if you dont have flights or customrs");
            }
                for (int i = 0; i < (ticket * customers); i++)
                {
                    List<FlightCenterProject.Ticket> tickets = new List<FlightCenterProject.Ticket>();

                    long flightId = flights[rnd.Next(flights.Count)].Id;

                    long customerId = customers2[rnd.Next(customers2.Count)].Id;
                    bool ThereIsAlreadyATicket = false;
                    foreach (var ticket1 in tickets)
                    {
                        if (ticket1.FlightId == flightId)
                        {
                            if (ticket1.CustomerId == customerId)
                            {
                                ThereIsAlreadyATicket = true;
                                break;
                            }

                        }
                    }
                    if (ThereIsAlreadyATicket == false)
                    {
                        FlightCenterProject.TicketDAOMSSQL ticketDAOMSSQL = new FlightCenterProject.TicketDAOMSSQL();
                        FlightCenterProject.Ticket ticket1 = new FlightCenterProject.Ticket(flightId, customerId);
                        ticketDAOMSSQL.Add(ticket1);
                    }                 
                   
                    Percent += Convert.ToInt32(1.0 / Total * 100);
                   
                    Message = $"{i + 1}/{ticket * customers} Tickets created";
                }

            }catch(YouNeedToHaveFlightsAndCustomersToGetTicketsException e)
            {
                Message = e.Message;
            }
        }

        /// <summary>
        /// Delete all the database.
        /// </summary>
        public void DeleteFromDB()
        {
            FlightCenterProject.CustomerDAOMSSQL customerDAOMSSQL = new FlightCenterProject.CustomerDAOMSSQL();
            customerDAOMSSQL.RemoveAll();
            FlightCenterProject.FlightDAOMSSQL flightDAOMSSQL = new FlightCenterProject.FlightDAOMSSQL();
            flightDAOMSSQL.RemoveAll();
            FlightCenterProject.AirlineCompanyDAOMSSQL airlineCompanyDAOMSSQL = new FlightCenterProject.AirlineCompanyDAOMSSQL();
            airlineCompanyDAOMSSQL.RemoveAll();
            FlightCenterProject.CountryDAOMSSQL countryDAOMSSQL = new FlightCenterProject.CountryDAOMSSQL();
            countryDAOMSSQL.RemoveAll();
            Message = "The entire database has been deleted";
        }


    }
}
