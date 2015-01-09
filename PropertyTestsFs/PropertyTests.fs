module PropertyTests

open FsCheck
open FsCheck.NUnit
open Code
open System.Linq

let myConfig = Config.QuickThrowOnFailure

let unitPrice = PotterCalculator.UnitPrice

let checkPriceOfBooks titles expectedPrice =
    let actualPrice = PotterCalculator.CalculatePrice titles
    actualPrice = expectedPrice

let checkPriceOfBooksSpec gen expectedPriceFun =
    let arb = Arb.fromGen gen
    let property = Prop.forAll arb (fun titles -> checkPriceOfBooks titles (expectedPriceFun titles))
    Check.One(myConfig, property)

let checkPriceOfBooksSpecWithFixedPrice gen expectedPrice =
    checkPriceOfBooksSpec gen (fun _ -> expectedPrice)

let titlesAreDifferent titles =
    Seq.length(Seq.distinct titles) = Seq.length titles

let combinationsOfTwoDifferentTitles all =
    seq {
        for title1 in all do
        for title2 in all do
        let titles = [|title1; title2|]
        if titlesAreDifferent(titles) then yield titles
    }

let combinationsOfThreeDifferentTitles all =
    seq {
        for title1 in all do
        for title2 in all do
        for title3 in all do
        let titles = [|title1; title2; title3|]
        if titlesAreDifferent(titles) then yield titles
    }

let combinationsOfFourDifferentTitles all =
    seq {
        for title1 in all do
        for title2 in all do
        for title3 in all do
        for title4 in all do
        let titles = [|title1; title2; title3; title4|]
        if titlesAreDifferent(titles) then yield titles
    }

let combinationsOfFiveDifferentTitles all =
    seq {
        for title1 in all do
        for title2 in all do
        for title3 in all do
        for title4 in all do
        for title5 in all do
        let titles = [|title1; title2; title3; title4; title5|]
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

let genFullyOverlappingFourDifferentTitlesPlusTwoDifferentTitles =
    gen {
        let! four = genFourDifferentTitles
        let! two = Gen.elements(combinationsOfTwoDifferentTitles(four))
        return Array.append four two
    }

[<Property>]
let ``one book``() = 
    let expectedPrice = unitPrice
    checkPriceOfBooksSpecWithFixedPrice genOneTitleArray expectedPrice

[<Property>]
let ``multiple books the same``() = 
    let expectedPriceFun = fun titles -> (Seq.length titles |> decimal) * unitPrice
    checkPriceOfBooksSpec genMultipleTitlesTheSame expectedPriceFun

[<Property>]
let ``two books different``() = 
    let expectedPrice = 2m * unitPrice * 0.95m
    checkPriceOfBooksSpecWithFixedPrice genTwoDifferentTitles expectedPrice

[<Property>]
let ``three books different``() = 
    let expectedPrice = 3m * unitPrice * 0.90m
    checkPriceOfBooksSpecWithFixedPrice genThreeDifferentTitles expectedPrice

[<Property>]
let ``four books different``() = 
    let expectedPrice = 4m * unitPrice * 0.80m
    checkPriceOfBooksSpecWithFixedPrice genFourDifferentTitles expectedPrice

[<Property>]
let ``five books different``() = 
    let expectedPrice = 5m * unitPrice * 0.75m
    checkPriceOfBooksSpecWithFixedPrice genFiveDifferentTitles expectedPrice

[<Property>]
let ``fully overlapping four different books plus two different books``() =
    let gen = genFullyOverlappingFourDifferentTitlesPlusTwoDifferentTitles
    let expectedPrice = (4m * unitPrice * 0.80m) + (2m * unitPrice * 0.95m)
    checkPriceOfBooksSpecWithFixedPrice gen expectedPrice
