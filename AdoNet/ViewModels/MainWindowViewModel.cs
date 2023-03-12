using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media.Animation;
using AdoNet.Infrastructure;
using EFAccess;
using EFAccess.Models;
using EFSQL;
using EFSQL.Model;
using Microsoft.EntityFrameworkCore;

namespace AdoNet.ViewModels
{
    internal class MainWindowViewModel : INPC, INotifyDataErrorInfo
    {
        #region Поля и свойства

        #region Словарь ошибок
        private readonly Dictionary<string, List<string>> propertyErrors = new Dictionary<string, List<string>>();

        #endregion


        #region Статус SQL соединения
        private string sqlConnectionStatus= ConnectionState.Closed.ToString();
        public string SqlConnectionStatus
        {
            get => sqlConnectionStatus;
            set => Set(ref sqlConnectionStatus, value);
        }
        #endregion

        #region EF SQL
        private DALsqlDB sqlEf;
        #endregion

        #region SQL строка соединения
        private string sqlConnectionsString; 
        public string SQLConnectionString 
        {
            get => sqlConnectionsString;
            set => Set(ref sqlConnectionsString, value); 
        }
        #endregion

        #region Access EF
        private DALAccessDB accessDBEF;
        #endregion

        #region Access Connection string
        private string accessConnectionsString;
        public string AccessConnectionString
        {
            get => accessConnectionsString;
            set => Set(ref accessConnectionsString, value);
        }
        #endregion

        #region Access Connection status
        private string accessConnectionStatus = ConnectionState.Closed.ToString();
        public string AccessConnectionStatus
        {
            get => accessConnectionStatus;
            set => Set(ref accessConnectionsString, value);
        }

        #endregion

        #region Данные о клиентах
        private BindingList<Clients> clients;
        public BindingList<Clients> Clients
        {
            get => clients;
            set => Set(ref clients, value);
        }
        #endregion

        #region Выбранный клиент
        private Clients selectedClient;
        public Clients SelectedClient
        {
            get => selectedClient;
            set
            {
                Set(ref selectedClient, value);
                if (value != null)
                {
                    GetSelectedClientPurchases(value.EMail);
                }
                if (value == null)
                {
                    Purchases = null;
                }
                
            }
        }
        #endregion

        #region Новый клиент
        private string clientSurname;
        public string ClientSurname
        {
            get => clientSurname;
            set
            {
                ClearErrors(nameof(ClientSurname));
                if (String.IsNullOrEmpty(value))
                {
                    AddError(nameof(ClientSurname), "Фамилия не может быть пустой");
                }
                if (!value.All(Char.IsLetter))
                {
                    AddError(nameof(ClientSurname), "Фамилия должна содержать только буквы");
                }
                Set(ref clientSurname, value);
            }
        }

        private string clientName;
        public string ClientName
        {
            get => clientName;
            set 
            {
                ClearErrors(nameof(ClientName));
                if (String.IsNullOrEmpty(value))
                {
                    AddError(nameof(ClientName), "Имя не может быть пустым");
                }
                if (!value.All(Char.IsLetter))
                {
                    AddError(nameof(ClientName), "Имя должно содержать только буквы");
                }
                Set(ref clientName, value);
            }
        }

        private string clientPatronymic;
        public string ClientPatronymic
        {
            get => clientPatronymic;
            set
            {
                ClearErrors(nameof(ClientPatronymic));
                if (String.IsNullOrEmpty(value))
                {
                    AddError(nameof(ClientPatronymic), "Отчество не может быть пустым");
                }
                if (!value.All(Char.IsLetter))
                {
                    AddError(nameof(ClientPatronymic), "Отчество должно содержать только буквы");
                }
                Set(ref clientPatronymic, value);
            }
        }

        private string eMail;
        public string EMail
        {
            get => eMail;
            set
            {
                ClearErrors(nameof(EMail));
                if (String.IsNullOrEmpty(value))
                {
                    AddError(nameof(EMail), "Почта не может быть пустым");
                }
                Set(ref eMail, value);
            }
        }

        private string phone;
        public string Phone 
        {
            get => phone; 
            set
            {
                ClearErrors();
                string pattern = @"^[0-9][0-9][0-9]-[0-9][0-9]-[0-9][0-9]$";
                if (!Regex.IsMatch(value, pattern, RegexOptions.None))
                {
                    AddError(nameof(Phone), "Введите телефон в формате ХХХ-ХХ-ХХ");
                }
                if (String.IsNullOrEmpty(value))
                {
                    ClearErrors();
                }

                Set(ref phone, value);
            }
        }

        #endregion

        #region Данные о покупках
        private List<Purchases> purchases;
        public List<Purchases> Purchases
        {
            get => purchases;
            set => Set(ref purchases, value);
        }
        #endregion

        #region Свойства для новой покупки
        private string itemCode;
        public string ItemCode
        {
            get => itemCode;
            set
            {
                ClearErrors();
                if (!value.All(Char.IsDigit))
                {
                    AddError(nameof(ItemCode), "Код товара может быть только числом");
                }
                Set(ref itemCode, value);

            }
        }

        private string itemName;
        public string ItemName
        {
            get => itemName;
            set
            {
                ClearErrors();
                if (String.IsNullOrEmpty(value))
                {
                    AddError(nameof(ItemName), "Наименование товара не может быть пустым");
                }
                Set(ref itemName, value);
            }
            
        }

        #endregion


        #region Статус обработки запроса
        private string requestStatus;

        public string RequestStatus
        {
            get => requestStatus;
            set => Set(ref requestStatus, value);
        }
        #endregion
        #endregion

        public MainWindowViewModel()
        {
            SQLConnectionSet = new Command(OnSQLConnectionSetExecute,
                                           CanSQLConnectionSetExecute);

            GetAllClientsCommand = new Command(OnGetAllClientsCommandExecute,
                                               CanGetAllClientsCommandExecute);

            AddNewClientCommand = new Command(OnAddNewClientCommandExecute,
                                              CanAddNewClientCommandExecute);

            CellEditEndCommand = new Command(OnCellEditEndCommandExcute, null);

            ClientCellChangedCommand = new Command(OnClientCellChangedCommandExecute, null);

            DeleteClientRecordCommand = new Command(OnDeleteClientRecordExecute,
                                                    CanDeleteClientRecordCommandExecute);

            AddNewPurchase = new Command(OnAddNewPurchaseExecute,
                                         CanAddNewPurchaseExecute);

            sqlEf = new DALsqlDB();
            SQLConnectionString = sqlEf.Connection;
            sqlEf.SqlConnectionStateChange += SQLConnectionStatusChange;

            accessDBEF = new DALAccessDB();
            AccessConnectionString = accessDBEF.ConnectionString;
            accessDBEF.ConnectionStatus += AccessConnectionStatusChange;

        }

        private void SQLConnectionStatusChange(string status)
        {
            SqlConnectionStatus = status;
        }
        private void AccessConnectionStatusChange(string status)
        {
            AccessConnectionStatus = status;
        }
        private void GetSelectedClientPurchases(string clientEmail)
        {
            Purchases = (accessDBEF.GetAllPurchases().Where(p => p.EMail == clientEmail)).ToList();
        }

        #region Команды

        #region Установить соединение
        public ICommand SQLConnectionSet { get; }
        private void OnSQLConnectionSetExecute(object p)
        {
             
        }
        private bool CanSQLConnectionSetExecute(object p) => true;

        #endregion

        #region Получить клиентов
        public ICommand GetAllClientsCommand { get; }
        private void OnGetAllClientsCommandExecute(object p)
        {
            Clients = sqlEf.CetAllClients().Local.ToBindingList();
        }
        private bool CanGetAllClientsCommandExecute(object p)
            => SqlConnectionStatus == ConnectionState.Closed.ToString();

        #endregion

        #region Добавить клиента
        public ICommand AddNewClientCommand { get; }

        private void OnAddNewClientCommandExecute(object p)
        {
            EFSQL.Model.Clients newClient = new Clients();

            newClient.ClientName = this.ClientName;
            newClient.ClientPatronymic = this.ClientPatronymic;
            newClient.ClientSurname = this.ClientSurname;
            newClient.Phone = this.Phone;
            newClient.EMail = this.EMail;

            sqlEf.AddNewClient(newClient);
            this.OnGetAllClientsCommandExecute(null);
        }

        private bool CanAddNewClientCommandExecute(object p)
        {
            if ( (String.IsNullOrEmpty(ClientName) || String.IsNullOrEmpty(ClientSurname)
                || String.IsNullOrEmpty(ClientPatronymic) || String.IsNullOrEmpty(EMail)) )
            {
                return false;
            }

            if ( propertyErrors.ContainsKey(nameof(ClientName)) || propertyErrors.ContainsKey(nameof(ClientSurname))
                || propertyErrors.ContainsKey(nameof(ClientPatronymic)) || propertyErrors.ContainsKey(nameof(EMail)) )
            {
                return false;
            }

            return true;
        }

        #endregion

        #region Удалить клиента
        public ICommand DeleteClientRecordCommand { get; }

        private void OnDeleteClientRecordExecute(object p)
        {
            if (SelectedClient != null)
            {
                sqlEf.DeleteClient(SelectedClient);
                this.OnGetAllClientsCommandExecute(null);
            }
            
        }

        private bool CanDeleteClientRecordCommandExecute(object p) => p != null;
        #endregion

        #region Добавить покупку

        public ICommand AddNewPurchase { get; }
        private void OnAddNewPurchaseExecute(object p)
        {
            EFAccess.Models.Purchases newPurchase = new Purchases();
            newPurchase.ItemName = this.ItemName;
            newPurchase.ItemCode = int.Parse(this.ItemCode);
            newPurchase.EMail = SelectedClient.EMail;

            accessDBEF.AddNewPurchase(newPurchase);

            this.GetSelectedClientPurchases(newPurchase.EMail);

        }
        private bool CanAddNewPurchaseExecute(object p) 
            => (p != null) && (!String.IsNullOrEmpty(ItemCode)) 
            && !propertyErrors.ContainsKey(nameof(ItemCode)) && (!String.IsNullOrEmpty(ItemName));

        #endregion

        #region Prism - Event as Commands

        #region Изменение клиента по завершении редактирования ячейки
        public ICommand CellEditEndCommand { get; }

        private void OnCellEditEndCommandExcute(object p)
        {
            sqlEf.SaveChanges();
        }

        #endregion

        #region Изменение клиента при смене ячейки
        public ICommand ClientCellChangedCommand { get; }

        private void OnClientCellChangedCommandExecute(object p)
        {
            if (SelectedClient == null)
            {
                return;
            }
            sqlEf.SaveChanges();
        }


        #endregion

        #endregion

        #endregion

        #region INotifyDataErrorInfo

        public event EventHandler<DataErrorsChangedEventArgs>? ErrorsChanged;

        public bool HasErrors => propertyErrors.Any();

        public IEnumerable GetErrors(string? propertyName)
        {
            return propertyErrors.GetValueOrDefault(propertyName, null);
        }

        public void AddError(string propertyName, string errorMessage)
        {
            if (!propertyErrors.ContainsKey(propertyName))
            {
                propertyErrors.Add(propertyName, new List<string>());
            }

            propertyErrors[propertyName].Add(errorMessage);
            OnErrorsChanged(propertyName);
        }

        private void ClearErrors([CallerMemberName]string propertyName = null)
        {
            if (propertyErrors.ContainsKey(propertyName))
            {
                propertyErrors.Remove(propertyName);
                OnErrorsChanged(propertyName);
            }
        }

        private void OnErrorsChanged(string propertyName)
        {
            ErrorsChanged?.Invoke(this, new DataErrorsChangedEventArgs(propertyName));
        }
        #endregion

    }

}
