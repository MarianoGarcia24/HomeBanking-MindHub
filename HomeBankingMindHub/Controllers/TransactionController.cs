using HomeBankingMindHub.DTOs;
using HomeBankingMindHub.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace HomeBankingMindHub.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TransactionController : ControllerBase
    {
        [HttpPost]
        [Authorize (Policy = "ClientOnly")]
        public Task<IActionResult> Post(TransactionDTO NewTransaction)
        {
            //VERIFICAR QUE LA CUENTA ORIGEN PERTENECE AL CLIENTE AUTENTICADO.


            //DEVUELVE:
            //201 created si se pudo hacer la transaccion
            //403 forbidden si el monto o descripcion estan vacios 
            //403 forbidden si alguno de los numeros de cuenta estan vacios
            //403 forbidden si la cuenta de origen no existe
            //403 forbidden, si la cuenta de destino no existe
            //403 forbidden, si la cuenta de origen no pertenece al cliente autenticado
            //403 forbidden, si el cliente no tiene fondos
            //403 forbidden, si la cuenta de origen es la misma que la destino
        }
    }
}
