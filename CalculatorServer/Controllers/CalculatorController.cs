using CalculatorServer.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace CalculatorServer.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CalculatorController : ControllerBase
    {
        private readonly ICalculatorService _calculator;

        public CalculatorController(ICalculatorService calculator)
        {
            _calculator = calculator;
        }

        [HttpPost]
        public IActionResult Calculate([FromBody] CalculationRequest expression)
        {
            try
            {
                _calculator.Calculate(expression.Expression);
                var result = _calculator.GetAns();
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("ComplexExpression")]
        public async Task<IActionResult> CalculateComplexExpression([FromBody] CalculationRequest expression)
        {
            try
            {
                var result = await _calculator.CalculateComplexExpression(expression.Expression);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }

    public class CalculationRequest
    {
        public string Expression { get; set; }
    }

}
