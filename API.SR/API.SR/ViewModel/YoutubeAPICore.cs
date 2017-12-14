using API.SR.Models;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Services;
using Google.Apis.Util.Store;
using Google.Apis.YouTube.v3;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Web;

namespace API.SR.ViewModel
{
    public class YoutubeAPICore
    {        
        // Objeto que irá receber o endpoint da API do Youtube já conectada via Oauth
        private static YouTubeService servicoYoutube = Auth();

        private static YouTubeService Auth()
        {
            // Método resposável por fazer o login no google e recuperar o token via Oauth 2.0
            UserCredential credenciais;
            using (var stream = new FileStream(HttpContext.Current.Server.MapPath(@"~\Json\youtube_client_secret.json"), FileMode.Open, FileAccess.Read))
            {
                credenciais = GoogleWebAuthorizationBroker.AuthorizeAsync(
                             GoogleClientSecrets.Load(stream).Secrets,
                             new[] { YouTubeService.Scope.YoutubeReadonly },
                             "user",
                             CancellationToken.None,
                             new FileDataStore("YoutubeAPI")
                    ).Result;
            }

            // Realizo a conexão com a API do Youtube através das credencias recuperadas acima
            var servico = new YouTubeService(new BaseClientService.Initializer()
            {
                HttpClientInitializer = credenciais,
                ApplicationName = "YoutubeAPI"
            });

            return servico;
        }

        //Método responsável por executar os métodos internos da API do Youtube e recuperar os dados do canal
        public static DadosCanalYoutube GetCanalYoutube()
        {
            var canal = servicoYoutube.Channels.List("contentDetails");
            canal.Mine = true;
            var retornoCanal = canal.Execute();

            DadosCanalYoutube dadosCanalYoutube = new DadosCanalYoutube
            {
                PlayListId = retornoCanal.Items[0].ContentDetails.RelatedPlaylists.Uploads,
                ChannelId = retornoCanal.Items[0].Id
            };

            return dadosCanalYoutube;
        }

        //Método responsável por executar os métodos internos da API do Youtube e recuperar os dados dos videos sem filtros
        public static IList<DadosYoutube> GetVideoYoutube(string playListId)
        {
            var listaVideosCanal = servicoYoutube.PlaylistItems.List("contentDetails");
            listaVideosCanal.PlaylistId = playListId;
            listaVideosCanal.MaxResults = 2;

            var retornoListaVideosCanal = listaVideosCanal.Execute();

            IList<DadosYoutube> dadosYoutube = new List<DadosYoutube>();

            foreach (var listaVideos in retornoListaVideosCanal.Items)
            {
                dadosYoutube.Add(new DadosYoutube()
                {
                    Id = listaVideos.ContentDetails.VideoId,
                    DataPublicacao = listaVideos.ContentDetails.VideoPublishedAt
                });
            }

            return dadosYoutube;
        }

        //Método responsável por executar os métodos internos da API do Youtube e recuperar os dados dos videos com filtros
        public static IList<DadosYoutube> GetVideoYoutube(string channelId, string dataInicial, string dataFinal)
        {
            var lista = servicoYoutube.Search.List("snippet");
            lista.ChannelId = channelId;
            lista.MaxResults = 50;
            lista.PublishedAfter = DateTime.Parse(dataInicial);
            lista.PublishedBefore = DateTime.Parse(dataFinal);

            var retornoLista = lista.Execute();

            IList<DadosYoutube> dadosYoutube = new List<DadosYoutube>();

            foreach (var listaVideos in retornoLista.Items.Reverse())
            {
                dadosYoutube.Add(new DadosYoutube()
                {
                    Id = listaVideos.Id.VideoId,
                    DataPublicacao = listaVideos.Snippet.PublishedAt
                });
            }

            return dadosYoutube;
        }
    }
}