namespace SpecflowCalculator.Specs.StepDefinitions;
public class Calculator
{
	public int FirstNumber { get; set; }
	public int SecondNumber { get; set; }

	public int Add()
	{
		return FirstNumber + SecondNumber;
	}
}

[Binding]
public sealed class CalculatorStepDefinitions
{
	// For additional details on SpecFlow step definitions see https://go.specflow.org/doc-stepdef
	private readonly Calculator calculator = new Calculator();


	[Given("the first number is (.*)")]
	public void GivenTheFirstNumberIs(int number)
	{
		calculator.FirstNumber = number;
	}

	[Given("the second number is (.*)")]
	public void GivenTheSecondNumberIs(int number)
	{
		calculator.SecondNumber = number;
	}

	[When("the two numbers are added")]
	public void WhenTheTwoNumbersAreAdded()
	{
		calculator.Add();
	}

	[Then("the result should be (.*)")]
	public void ThenTheResultShouldBe(int result)
	{
		//TODO: implement assert (verification) logic
		result.Should().Be(calculator.FirstNumber + calculator.SecondNumber);
	}
}
