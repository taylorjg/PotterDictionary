module PropertyTestsFs

open FsCheck
open FsCheck.NUnit
open FsCheck.Fluent
open Code

type public Tests() =

    let checkPriceOfBook (title:string) (expectedPrice:decimal) =
        let actualPrice = PotterCalculator.CalculatePrice title
        actualPrice = expectedPrice

    let genOneBook = Gen.elements(HarryPotterBooks.Titles)

    [<Property(Verbose=true)>]
    member public this.OneBook() = 
        let specBuilder = Spec.For (genOneBook, fun title -> checkPriceOfBook title 8m)
        specBuilder.QuickCheckThrowOnFailure()
