
## Description

During an interview recently, I paired with a candidate on the Harry
Potter kata. The candidate elected to use a dictionary to store the collection of books.
I have not seen this kata done using a dictionary before and I was curious to see how it would
turn out. In the end, we only got the first 2 or 3 simple tests finished. So, I decided to
give it go on my own. I am using an immutable dictionary where the key is the book title
and the value is the quantity of the book.

NOTE: I have not catered for the edge cases.

## Links

* [Harry Potter Kata](http://codingdojo.org/cgi-bin/index.pl?KataPotter)
* [System.Collections.Immutable NuGet Package](http://www.nuget.org/packages/System.Collections.Immutable/1.1.33-beta)

## Oddity Regarding Immutable Collections and NUnit

After switching from a <code>Dictionary&lt;string, int&gt;</code> to an <code>ImmutableDictionary&lt;string, int&gt;</code>, 
I encountered a problem running the unit tests and I have not been able to resolve it.
They all started to fail with a System.InvalidProgramException containing the message "Common Language Runtime detected an invalid program."
in System.Collections.Immutable.SecurePooledObject`1.Use.
Here is an example:

![Screenshot](https://raw.githubusercontent.com/taylorjg/PotterDictionary/master/Images/ImmutableCollectionsAndNUnitError.png "Screenshot")

However, the tests run fine if I select "Debug Unit Tests" (with or without any breakpoints set) instead of "Run Unit Tests".
The console test program runs fine too. I have no idea why the above error occurs in the context of NUnit - it is a complete mystery!

UPDATE: There is a bug report of a similar issue here:

https://github.com/dotnet/corefx/issues/320
