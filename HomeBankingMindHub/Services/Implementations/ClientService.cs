﻿using HomeBankingMindHub.DTOs;
using HomeBankingMindHub.Models;
using HomeBankingMindHub.Models.utils;
using HomeBankingMindHub.Repositories.Implementation;
using HomeBankingMindHub.Repositories.Interfaces;
using HomeBankingMindHub.Services.Interfaces;
using Microsoft.IdentityModel.Tokens;
using System.Net;
using System.Security.Cryptography;
using System.Text;

namespace HomeBankingMindHub.Services.Implementations
{
    public class ClientService : IClientService
    {
        private readonly IClientRepository _clientRepository;
        private readonly IAccountRepository _accountRepository;
        private readonly ICardRepository _cardRepository;

        public ClientService(IClientRepository clientRepository, IAccountRepository accountRepository, ICardRepository cardRepository)
        {
            _clientRepository = clientRepository;
            _accountRepository = accountRepository;
            _cardRepository = cardRepository;
        }


        public IEnumerable<ClientDTO> GetAll()
        {
            return _clientRepository.GetAll().Select(c => new ClientDTO(c)).ToList();
        }

        public Response GetClientByEmail(string email)
        {
            Client cl = _clientRepository.FindByEmail(email);
            if (cl == null)
                return new Response(HttpStatusCode.NotFound, "El cliente no existe en la base de datos");
            return new Response(HttpStatusCode.OK, new ClientDTO(cl));
        }

        public Response GetClientById(long clientId)
        {
            Client cl = _clientRepository.FindById(clientId);
            if (cl == null)
                return new Response(HttpStatusCode.BadRequest, "No se encontro el cliente solicitado");
            return new Response(HttpStatusCode.OK, new ClientDTO(cl));
        }

        private Client FindClientByEmail(string email)
        {
            Client cl = _clientRepository.FindByEmail(email);
            if (cl == null)
                throw new NullReferenceException("El cliente no existe en la base de datos");
            return cl;

        }


        public Response CreateClient(ClientSignUpDTO signUpDTO)
        {
            //Valido los datos de entrada
            if (!ValidateEntries(signUpDTO))
                return new Response(HttpStatusCode.BadRequest, "Datos de creacion invalidos. Corrija los errores y reintente nuevamente");
            //Valido si el email no esta en uso
            if (!ValidateEmail(signUpDTO.Email))
                return new Response(HttpStatusCode.Forbidden, "El mail ya se encuentra en uso. Pruebe con uno nuevo");

            //Lo creo
            Client cl = new Client
            {
                Email = signUpDTO.Email,
                Password = GetHashCode(signUpDTO.Password),
                FirstName = signUpDTO.FirstName,
                LastName = signUpDTO.LastName
            };

            //Llamo al repositorio para guardarlo
            SaveClient(cl);
            cl = FindClientByEmail(cl.Email);
            string AccountNumber = GenerateNewAccountNumber();

            Account acc = new Account
            {
                Balance = 0,
                ClientID = cl.Id,
                Number = "VIN" + AccountNumber,
                CreationDate = DateTime.Now,
            };
            _accountRepository.Save(acc);

            cl = FindClientByEmail(cl.Email);
            AccountClientDTO account = new AccountClientDTO(_accountRepository.FindByAccountNumber(acc.Number));
            return new Response(HttpStatusCode.Created, new ClientAccountDTO(cl, account));
        }

        public Response ValidateCredentials(ClientLoginDTO clientLoginDTO)
        {
            Client cl = FindClientByEmail(clientLoginDTO.Email);
            if (cl == null)
                return new Response(HttpStatusCode.Unauthorized, "Credenciales invalidas");

            if (!VerifyPassword(clientLoginDTO.Password, cl.Password))
                return new Response(HttpStatusCode.Unauthorized, "La contraseña es incorrecta. Reintente nuvamente");
            //check password with sha256    

            return new Response(HttpStatusCode.OK, new ClientDTO(cl));
        }

        public Response GetAccountsByClient(string email)
        {
            Client cl = _clientRepository.FindByEmail(email);
            if (cl != null)
            {
                return new Response(HttpStatusCode.OK, _accountRepository
                                                    .FindAccountsByClient(cl.Id)
                                                    .Select(c => new AccountDTO(c)).ToList());
            }
            return new Response(HttpStatusCode.Forbidden, "El cliente no existe en la base de datos");
        }

        public Response CreateNewAccount(string email)
        {
            Client cl = _clientRepository.FindByEmail(email);
            var clAccounts = _accountRepository.FindAccountsByClient(cl.Id);
            if (clAccounts.Count() == 3)
            {
                throw new InvalidOperationException("Numero de cuentas máximo alcanzado. El cliente posee 3 cuentas.");
            }

            string acNumber = GenerateNewAccountNumber();
            Account acc = new Account
            {
                Balance = 0,
                Number = "VIN" + acNumber,
                ClientID = cl.Id,
                CreationDate = DateTime.Now,
            };
            _accountRepository.Save(acc);
            Account acc2 = _accountRepository.FindByAccountNumber(acc.Number);
            return new Response(HttpStatusCode.Created, new AccountDTO(acc2));
        }

         public void SaveClient(Client client)
        {
            _clientRepository.Save(client);
        }



        private bool ValidateEntries(ClientSignUpDTO signUpDTO)
        {
            if (string.IsNullOrEmpty(signUpDTO.Email) || string.IsNullOrEmpty(signUpDTO.Password) ||
                   string.IsNullOrEmpty(signUpDTO.FirstName) || string.IsNullOrEmpty(signUpDTO.LastName))
                return false;
            return true;
        }

        private bool ValidateEmail(string email)
        {
            if (_clientRepository.FindByEmail(email) != null)
                return false;
            return true;
        }

        private string GenerateNewAccountNumber()
        {
            string acNumber;
            do
            {
                acNumber = new Random().Next(1000, 100000000).ToString();
            } while (_accountRepository.FindByAccountNumber(acNumber) != null);
            return acNumber;
        }



        private static string GetHashCode(string Password)
        {
            using (SHA256 sha256 = SHA256.Create())
            {
                byte[] data = sha256.ComputeHash(Encoding.UTF8.GetBytes(Password));

                string sBuilder = Convert.ToBase64String(data);

                return sBuilder;
            }
        }

        private bool VerifyPassword(string Input, string ClientDBPassword)
        {
            if (string.IsNullOrEmpty(Input))
                return false;

            string HashedPassword = GetHashCode(Input);
            return string.Equals(ClientDBPassword.ToLower(), HashedPassword.ToLower());
        }


    }
}
