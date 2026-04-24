This API analyzes the data of  Insighta Labs, a demographic intelligence company. All has been done by using ASP Web API.
Therefore there are two endpoints: one for filtering, sorting, pagination and other for natural language processing.
As for the natural language processing, it hardly works because it does not use AI. But here is how it works:
The keywords supported are young, teenager, child, adult, senior, male, female, above, below, and. word people is ignored and the other words would be 
considered as names of countries.
Here is the process:
I break down each query into words. Each word would give some or all of these pieces of information of a criterion: left expression, comparator(=, <...),
right expression, logical operator(or, and). Then I assemble these criteria in a string. The filter then filters the data accordingly 
and I use dynamic LINQ in order to use the filter string.
But this is clearly limited. If you use some keywords other than those used, it may not work. 
