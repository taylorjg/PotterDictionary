module PotterDictionaryPropertyTests

open NUnit.Framework
open FsCheck
open FsCheck.NUnit
open FsCheck.Fluent
open System.Linq
open Code

let unitPrice = PotterCalculator.UnitPrice

let checkPriceOfBooks (titles:string[]) (expectedPrice:decimal) =
    let actualPrice = PotterCalculator.CalculatePrice titles
    actualPrice = expectedPrice

let checkPriceOfBooksSpec (gen:Gen<string[]>) (expectedPriceFun:string[] -> decimal) =
    let specBuilder = Spec.For (gen, fun titles -> checkPriceOfBooks titles (expectedPriceFun titles))
    specBuilder.QuickCheckThrowOnFailure()

let fixedExpectedPriceFun (expectedPrice:decimal) = 
    fun _ -> expectedPrice

let titlesAreDifferent titles =
    Seq.length(Seq.distinct titles) = Seq.length titles

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
let genOneTitleArray = Gen.map (fun title -> Seq.singleton title |> Seq.toArray) genOneTitle
let genTwoDifferentTitles = Gen.elements(combinationsOfTwoDifferentTitles(HarryPotterBooks.Titles))
let genThreeDifferentTitles = Gen.elements(combinationsOfThreeDifferentTitles(HarryPotterBooks.Titles))
let genFourDifferentTitles = Gen.elements(combinationsOfFourDifferentTitles(HarryPotterBooks.Titles))
let genFiveDifferentTitles = Gen.elements(combinationsOfFiveDifferentTitles(HarryPotterBooks.Titles))

let genMultipleTitlesTheSame =
    gen {
        let! title = genOneTitle
        let! n = Gen.choose(0, 20)
        return Enumerable.Repeat(title, n) |> Seq.toArray
    }

let genOverlappingFourDifferentTitlesPlusTwoDifferentTitles =
    gen {
        let! four = genFourDifferentTitles
        let! two = Gen.elements(combinationsOfTwoDifferentTitles(four))
        return Array.append four two
    }

type MyArbitraries =
  static member NonShrinkingStringArray() =
      { new Arbitrary<string[]>() with
          override x.Generator = Gen.constant [||]
          override x.Shrinker _ = Seq.empty }    

[<SetUp>]
let setUp =
    DefaultArbitraries.Add<MyArbitraries>() |> ignore

[<Property>]
let ``one book``() = 
    let expectedPrice = unitPrice
    checkPriceOfBooksSpec genOneTitleArray (fixedExpectedPriceFun expectedPrice)

[<Property>]
let ``multiple books the same``() = 
    let expectedPriceFun = fun titles -> (Seq.length titles |> decimal) * unitPrice
    checkPriceOfBooksSpec genMultipleTitlesTheSame expectedPriceFun

[<Property>]
let ``two books different``() = 
    let expectedPrice = 2m * unitPrice * 0.95m
    checkPriceOfBooksSpec genTwoDifferentTitles (fixedExpectedPriceFun expectedPrice)

[<Property>]
let ``three books different``() = 
    let expectedPrice = 3m * unitPrice * 0.90m
    checkPriceOfBooksSpec genThreeDifferentTitles (fixedExpectedPriceFun expectedPrice)

[<Property>]
let ``four books different``() = 
    let expectedPrice = 4m * unitPrice * 0.80m
    checkPriceOfBooksSpec genFourDifferentTitles (fixedExpectedPriceFun expectedPrice)

[<Property>]
let ``five books different``() = 
    let expectedPrice = 5m * unitPrice * 0.75m
    checkPriceOfBooksSpec genFiveDifferentTitles (fixedExpectedPriceFun expectedPrice)

[<Property>]
let ``overlapping four different books plus two different books``() =
    let expectedPrice = (4m * unitPrice * 0.80m) + (2m * unitPrice * 0.95m)
    checkPriceOfBooksSpec genOverlappingFourDifferentTitlesPlusTwoDifferentTitles (fixedExpectedPriceFun expectedPrice)
