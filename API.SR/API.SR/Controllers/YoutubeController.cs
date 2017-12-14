using API.SR.Models;
using API.SR.ViewModel;
using System.Collections.Generic;
using System.Web.Http;

namespace API.SR.Controllers
{
    //[ApiVersion()]
    [RoutePrefix("Api/SR/Youtube")]
    public class YoutubeController : ApiController
    {
        /// <summary>
        /// Recupera os dados do canal do youtube
        /// </summary>
        /// <returns></returns>
        [HttpGet, Route("DadosCanal")]
        public DadosCanalYoutube GetDadosCanalYoutube()
        {
            return YoutubeAPICore.GetCanalYoutube();
        }

        /// <summary>
        /// Recupera os dados do canal do youtube sem filtros
        /// </summary>
        /// <param name="playListId"></param>
        /// <returns></returns>
        [HttpGet, Route("{playListId}/DadosVideo")]
        public IList<DadosYoutube> GetDadosVideoYoutube(string playListId) => YoutubeAPICore.GetVideoYoutube(playListId);

        /// <summary>
        /// Recupera os dados dos videos do youtube com filtros
        /// </summary>
        /// <param name="channelId"></param>
        /// <param name="dataInicial"></param>
        /// <param name="dataFinal"></param>
        /// <returns></returns>
        [HttpGet, Route("{channelId}/DadosVideo")]
        public IList<DadosYoutube> GetDadosVideoYoutube(string channelId, string dataInicial, string dataFinal) 
            => YoutubeAPICore.GetVideoYoutube(channelId, dataInicial, dataFinal);
    }
}