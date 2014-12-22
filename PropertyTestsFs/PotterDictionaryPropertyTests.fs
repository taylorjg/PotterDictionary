module PotterDictionaryPropertyTests

open NUnit.Framework
open FsCheck
open FsCheck.NUnit
open FsCheck.Fluent
open System.Linq
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

let combinationsOfFourDifferentTitles all =
    seq {
        for t1 in all do
        for t2 in all do
        for t3 in all do
        for t4 in all do
        let titles = [|t1; t2; t3; t4|]
        if titlesAreDifferent(titles) then yield titles
    }

let combinationsOfFiveDifferentTitles all =
    seq {
        for t1 in all do
        for t2 in all do
        for t3 in all do
        for t4 in all do
        for t5 in all do
        let titles = [|t1; t2; t3; t4; t5|]
        if titlesAreDifferent(titles) then yield titles
    }

let genOneTitle = Gen.elements(HarryPotterBooks.Titles)
let genTwoDifferentTitles = Gen.elements(combinationsOfTwoDifferentTitles(HarryPotterBooks.Titles))
let genThreeDifferentTitles = Gen.elements(combinationsOfThreeDifferentTitles(HarryPotterBooks.Titles))
let genFourDifferentTitles = Gen.elements(combinationsOfFourDifferentTitles(HarryPotterBooks.Titles))
let genFiveDifferentTitles = Gen.elements(combinationsOfFiveDifferentTitles(HarryPotterBooks.Titles))

let genMultipleTitlesTheSame =
    gen {
        let! title = genOneTitle
        let! n = Gen.choose(1, 10)
        return Enumerable.Repeat(title, n) |> Seq.toArray
    }

let genOverlappingFourDifferentTitlesPlusTwoDifferentTitles =
    gen {
        let! four = genFourDifferentTitles
        let! two = Gen.elements(combinationsOfThreeDifferentTitles(four))
        return Array.append four two
    }

type MyArbitraries =
  static member NonShrinkingStringArray() =
      {new Arbitrary<string[]>() with
          override x.Generator = Gen.constant [||]
          override x.Shrinker _ = Seq.empty }    

[<SetUp>]
let setUp = DefaultArbitraries.Add<MyArbitraries>() |> ignore

[<Property(Verbose=true)>]
let ``one book``() = 
    let specBuilder = Spec.For (genOneTitle, fun title -> checkPriceOfBooks [|title|] 8m)
    specBuilder.QuickCheckThrowOnFailure()

[<Property(Verbose=true)>]
let ``multiple books the same``() = 
    let specBuilder = Spec.For (genMultipleTitlesTheSame, fun titles -> checkPriceOfBooks titles (8m * (Seq.length titles |> decimal)))
    specBuilder.QuickCheckThrowOnFailure()

[<Property(Verbose=true)>]
let ``two books different``() = 
    let specBuilder = Spec.For (genTwoDifferentTitles, fun titles -> checkPriceOfBooks titles (2m * 8m * 0.95m))
    specBuilder.QuickCheckThrowOnFailure()

[<Property(Verbose=true)>]
let ``three books different``() = 
    let specBuilder = Spec.For (genThreeDifferentTitles, fun titles -> checkPriceOfBooks titles (3m * 8m * 0.90m))
    specBuilder.QuickCheckThrowOnFailure()

[<Property(Verbose=true)>]
let ``four books different``() = 
    let specBuilder = Spec.For (genFourDifferentTitles, fun titles -> checkPriceOfBooks titles (4m * 8m * 0.80m))
    specBuilder.QuickCheckThrowOnFailure()

[<Property(Verbose=true)>]
let ``five books different``() = 
    let specBuilder = Spec.For (genFiveDifferentTitles, fun titles -> checkPriceOfBooks titles (5m * 8m * 0.75m))
    specBuilder.QuickCheckThrowOnFailure()

let ``overlapping four different books plus two different books``() =
    let expectedPrice = (4m * 8m * 0.80m) + (2m * 8m * 0.95m)
    let specBuilder = Spec.For (genOverlappingFourDifferentTitlesPlusTwoDifferentTitles, fun titles -> checkPriceOfBooks titles expectedPrice)
    specBuilder.QuickCheckThrowOnFailure()
