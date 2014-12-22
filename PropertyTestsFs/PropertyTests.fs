module PropertyTestsFs

open NUnit.Framework
open FsCheck
open FsCheck.NUnit
open FsCheck.Fluent
open Code

let checkPriceOfBooks (titles:string[]) (expectedPrice:decimal) =
    let actualPrice = PotterCalculator.CalculatePrice titles
    actualPrice = expectedPrice

let titlesAreDifferent (titles:string[]) =
    Seq.length(Seq.distinct(titles)) = Seq.length titles

let combinationsOfTwoDifferentTitles all =
    seq {
        for t1 in all do
        for t2 in all do
        let titles = [|t1; t2|]
        if titlesAreDifferent(titles) then yield titles
    }

let combinationsOfThreeDifferentTitles all =
    seq {
        for t1 in all do
        for t2 in all do
        for t3 in all do
        let titles = [|t1; t2; t3|]
        if titlesAreDifferent(titles) then yield titles
    }

type MyArbitraries =
  static member NonShrinkingStringArray() =
      {new Arbitrary<string[]>() with
          override x.Generator = Gen.constant [||]
          override x.Shrinker _ = Seq.empty }    

let genOneTitle = Gen.elements(HarryPotterBooks.Titles)
let genTwoDifferentTitles = Gen.elements(combinationsOfTwoDifferentTitles(HarryPotterBooks.Titles))
let genThreeDifferentTitles = Gen.elements(combinationsOfThreeDifferentTitles(HarryPotterBooks.Titles))

[<SetUp>]
let SetUp = DefaultArbitraries.Add<MyArbitraries>() |> ignore

[<Property(Verbose=true)>]
let ``one book``() = 
    let specBuilder = Spec.For (genOneTitle, fun title -> checkPriceOfBooks [|title|] 8m)
    specBuilder.QuickCheckThrowOnFailure()

[<Property(Verbose=true)>]
let ``two books different``() = 
    let specBuilder = Spec.For (genTwoDifferentTitles, fun titles -> checkPriceOfBooks titles (2m * 8m * 0.95m))
    specBuilder.QuickCheckThrowOnFailure()


[<Property(Verbose=true)>]
let ``three books different``() = 
    let specBuilder = Spec.For (genThreeDifferentTitles, fun titles -> checkPriceOfBooks titles (3m * 8m * 0.90m))
    specBuilder.QuickCheckThrowOnFailure()
