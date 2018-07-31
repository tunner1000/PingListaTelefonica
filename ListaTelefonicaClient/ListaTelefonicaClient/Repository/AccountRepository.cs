﻿using ListaTelefonicaClient.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace ListaTelefonicaClient.Repository
{
    /// <summary>
    /// Repositório de gerenciamento de contas de usuário
    /// </summary>
    public class AccountRepository : IAccountRepository
    {
        /// <summary>
        /// Era pra ser utilizado para controle de autenticação
        /// </summary>
        HttpClient _client;

        /// <summary>
        /// Uri correspondente à página de login
        /// </summary>
        const string _URI = "api/Login/";

        /// <summary>
        /// Inicializa uma nova instância de <see cref="AccountRepository"/>
        /// </summary>
        /// <param name="client">HttpClient</param>
        public AccountRepository(HttpClient client)
        {
            _client = client;
        }

        /// <summary>
        /// Fazendo login
        /// </summary>
        /// <param name="userName">Nome de usuário</param>
        /// <param name="pass">Senha</param>
        /// <param name="rememberMe">True para manter conectado, false caso contrário (Não implementado)</param>
        public async Task<(HttpResponseMessage httpResponse, Token token)> Login(string userName, string pass, bool rememberMe)
        {
            Token token = new Token();

            // Envio da requisição a fim de autenticar e obter o token de acesso
            HttpResponseMessage respToken = this._client.PostAsync(
                _URI, new StringContent(
                    JsonConvert.SerializeObject(new
                    {
                        UserID = userName,
                        Password = pass,
                        RememberMe = rememberMe
                    }), Encoding.UTF8, "application/json")).Result;

            string conteudo =
                respToken.Content.ReadAsStringAsync().Result;

            if (respToken.StatusCode == HttpStatusCode.OK)
            {
                token = JsonConvert.DeserializeObject<Token>(conteudo);
                if (token.Authenticated)
                {
                    // Associar o token aos headers do objeto do tipo HttpClient
                    this._client.DefaultRequestHeaders.Authorization =
                        new AuthenticationHeaderValue("Bearer", token.AccessToken);

                    List<Contato> contatos = new List<Contato>();
                    HttpResponseMessage res = _client.GetAsync("api/Contatos/").Result;

                    if (res.IsSuccessStatusCode)
                    {
                        var result = await res.Content.ReadAsStringAsync();
                        contatos = JsonConvert.DeserializeObject<List<Contato>>(result);
                    }
                    
                    return (httpResponse: respToken, token: token);
                }
            }

            return (httpResponse: respToken, token: token);
        }
    }
}