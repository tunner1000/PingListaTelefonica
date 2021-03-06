﻿using ListaTelefonicaClient.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace ListaTelefonicaClient.Repository
{
    /// <summary>
    /// Repositório para buscar mandar para a API requisições de contatos
    /// </summary>
    public class ContatosRepository : IContatosRepository
    {
        /// <summary>
        /// URI padrão dos contatos na Api
        /// </summary>
        private const string _URI = "api/Contatos/";

        /// <summary>
        /// Classe de httpClient
        /// </summary>
        private readonly HttpClient _client;

        /// <summary>
        /// Inicializa uma nova instância de <see cref="ContatosRepository"/>
        /// </summary>
        /// <param name="client">httpClient</param>
        public ContatosRepository(HttpClient client)
        {
            _client = client;
        }

        /// <summary>
        /// Criando novo contato
        /// </summary>
        /// <param name="contato">Contato a ser criado</param>
        public async Task<HttpResponseMessage> Create(ContatoViewModel contato)
        {
            try
            {
                var serialContato = JsonConvert.SerializeObject(contato);
                var content = new StringContent(serialContato, Encoding.UTF8, "application/json");

                var res = await PostAsync(_URI, content);

                return res;
            }
            catch (Exception ex)

            {
                throw ex;
            }
        }

        /// <summary>
        /// Removendo contato
        /// </summary>
        /// <param name="id">Código do contato</param>
        public async Task<HttpResponseMessage> Delete(int id)
        {
           return await DeleteAsync(_URI + id);
        }

        /// <summary>
        /// Obtendo contato por código
        /// </summary>
        /// <param name="id">Código do contato</param>
        /// <returns>Contato</returns>
        public async Task<ContatoViewModel> GetContatoAsync(int id)
        {
            var contato = new ContatoViewModel();

            HttpResponseMessage res = await GetAsync(_URI + id);

            if (res.IsSuccessStatusCode)
            {
                var result = await res.Content.ReadAsStringAsync();
                contato = JsonConvert.DeserializeObject<ContatoViewModel>(result);
            }

            return contato;
        }

        /// <summary>
        /// Obtendo todos os contatos
        /// </summary>
        /// <returns>Lista de contatos</returns>
        public async Task<IEnumerable<ContatoViewModel>> GetContatosAsync()
        {
            List<ContatoViewModel> contatos = new List<ContatoViewModel>();

            HttpResponseMessage res = await GetAsync(_URI);

            if (res.IsSuccessStatusCode)
            {
                var result = await res.Content.ReadAsStringAsync();
                contatos = JsonConvert.DeserializeObject<List<ContatoViewModel>>(result);
            }

            return contatos;
        }

        /// <summary>
        /// Obtendo contatos
        /// </summary>
        /// <param name="filtro">Filtro a ser aplicado no campo nome</param>
        /// <returns>Lista de contatos</returns>
        public async Task<List<ContatoViewModel>> GetContatosAsync(string filtro)
        {
            List<ContatoViewModel> contatos = new List<ContatoViewModel>();

            HttpResponseMessage res = await GetAsync(_URI + "?filtro=" + filtro);

            if (res.IsSuccessStatusCode)
            {
                var result = await res.Content.ReadAsStringAsync();
                contatos = JsonConvert.DeserializeObject<List<ContatoViewModel>>(result);
            }

            return contatos;
        }

        /// <summary>
        /// Atualizando o contato
        /// </summary>
        /// <param name="contato">Contato a ser atualizado</param>
        public async Task<HttpResponseMessage> Update(ContatoViewModel contato)
        {
            return CheckAuth(await PutAsJsonAsync(_URI + contato.Codigo, contato));
        }

        /// <summary>
        /// Deletando contato
        /// </summary>
        /// <param name="uri"></param>
        /// <returns></returns>
        private async Task<HttpResponseMessage> DeleteAsync(string uri)
        {
            return CheckAuth(await _client.DeleteAsync(uri));
        }

        /// <summary>
        /// Obtendo contatos
        /// </summary>
        /// <param name="uri">URL de acesso</param>
        private async Task<HttpResponseMessage> GetAsync(string uri)
        {
            return CheckAuth(await _client.GetAsync(uri));
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="uri"></param>
        /// <param name="content"></param>
        /// <returns></returns>
        private async Task<HttpResponseMessage> PostAsync(string uri, StringContent content)
        {
            return CheckAuth(await _client.PostAsync(uri, content));
        }

        /// <summary>
        /// Atualizando contato
        /// </summary>
        /// <param name="uri">URL de acesso</param>
        /// <param name="contato">Contato sendo atualizado</param>
        private async Task<HttpResponseMessage> PutAsJsonAsync(string uri, ContatoViewModel contato)
        {
            return CheckAuth(await _client.PutAsJsonAsync(uri, contato));
        }

        private HttpResponseMessage CheckAuth(HttpResponseMessage res)
        {
            if (!res.IsSuccessStatusCode && res.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                throw new NotAuthorizedException();

            return res;
        }
    }
}