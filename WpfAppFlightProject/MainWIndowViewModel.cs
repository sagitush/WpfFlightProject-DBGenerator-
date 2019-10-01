using Prism.Commands;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;

namespace WpfAppFlightProject
{
    public class MainWIndowViewModel : INotifyPropertyChanged
    {      
        public DelegateCommand AddToDBCommand { get; set; }
        public DelegateCommand ReplaceDBCommand { get; set; }
        public event PropertyChangedEventHandler PropertyChanged;
        public DBGenerator dBGenerator { get; set; }        
        private bool _isBusy = false;
        
        private int customers;
        public int Customers
        {
            get
            {
                return customers;
            }
            set
            {
                customers = value;
                OnPropertyChanged("Customers");
            }
        }

        private int countries;
        public int Countries
        {
            get
            {
                return countries;
            }
            set
            {
                countries = value;
                OnPropertyChanged("Countries");
            }
        }

        private int airlinesCom;
        public int AirlinesCom
        {
            get
            {
                return airlinesCom;
            }
            set
            {
                airlinesCom = value;
                OnPropertyChanged("AirlinesCom");
            }
        }

        private int flights;
        public int Flights
        {
            get
            {
                return flights;
            }
            set
            {
                flights = value;
                OnPropertyChanged("Flights");
            }
        }

        private int ticket;
        public int Ticket
        {
            get
            {
                return ticket;
            }
            set
            {
                ticket = value;
                OnPropertyChanged("Ticket");
            }
        }

        private double percent;
        public double Percent
        {
            get
            {
                return percent;
            }
            set
            {
                percent = value;
                OnPropertyChanged("Percent");
            }
        }

        private string message;
        public string Message
        {
            get
            {
                return message;
            }
            set
            {
                message = value;
                OnPropertyChanged("Message");
            }
        }


        public MainWIndowViewModel()
        {
            dBGenerator = new DBGenerator();
            AddToDBCommand = new DelegateCommand(Execute1, CanExecute);
            ReplaceDBCommand = new DelegateCommand(Execute2, CanExecute);
            Task.Run(() =>
            {
                while (true)
                {
                    AddToDBCommand.RaiseCanExecuteChanged();
                    ReplaceDBCommand.RaiseCanExecuteChanged();
                    Thread.Sleep(500);
                }
            });
        }

        public void OnPropertyChanged(string property)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(property));
            }
        }

        public async void Execute1()
        {
                percent =dBGenerator.Percent= 0;
            
                _isBusy = true;

                dBGenerator.Total = customers + airlinesCom + countries + (ticket * customers) + (flights * airlinesCom);
                
                await Task.Run(() =>
                {
                    dBGenerator.AddCustomersToDB(customers);
                    Percent = dBGenerator.Percent;
                    Message = dBGenerator.Message;
                    dBGenerator.AddCountriesToDB(countries);
                    Percent = dBGenerator.Percent;
                    Message = dBGenerator.Message;
                    dBGenerator.AddAirlinesToDB(AirlineCompanies.companies,airlinesCom);
                    Percent = dBGenerator.Percent;
                    Message = dBGenerator.Message;
                    dBGenerator.AddFlightsToDB(flights,airlinesCom);
                    Percent = dBGenerator.Percent;
                    Message = dBGenerator.Message;
                    dBGenerator.AddTicketsToDB(ticket,customers);
                    Percent = dBGenerator.Percent;
                    Message = dBGenerator.Message;
                    _isBusy = false;
                });
            
        }

        public async void Execute2()
        {
            percent = 0;

            _isBusy = true;

            dBGenerator.Total = customers + airlinesCom + countries + (ticket * customers) + (flights * airlinesCom);

            await Task.Run(() =>
            {

                dBGenerator.DeleteFromDB();
                Percent = dBGenerator.Percent = 0;
                Message = dBGenerator.Message;
                dBGenerator.AddCustomersToDB(customers);
                Percent = dBGenerator.Percent;
                Message = dBGenerator.Message;
                dBGenerator.AddCountriesToDB(countries);
                Percent = dBGenerator.Percent;
                Message = dBGenerator.Message;
                dBGenerator.AddAirlinesToDB(AirlineCompanies.companies, airlinesCom);
                Percent = dBGenerator.Percent;
                Message = dBGenerator.Message;
                dBGenerator.AddFlightsToDB(flights, airlinesCom);
                Percent = dBGenerator.Percent;
                Message = dBGenerator.Message;
                dBGenerator.AddTicketsToDB(ticket, customers);
                Percent = dBGenerator.Percent;
                Message = dBGenerator.Message;
                _isBusy = false;
            }
            );
        }
         
        public bool CanExecute()
        {
            return !_isBusy;
        }
    }
}
