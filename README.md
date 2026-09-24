This application is called bank account and it may be that at some point but right now it is more to keep track of expences and income.
It is a basic console application in C# where each posting is classified as an expence or an income and saved in list that is later saved in a JSON file upon exit
Next steps should be a better gui and using a database.
In terms of installing it just clone the repository and build it fx with Visual Studio. It will start with nothing so just start adding posts. It is pretty stable,
but still save once in a while and try out all the menuchoiches once you have entered a few postings. If you like you can find the JSON file in the default working
directory and modify it directly and then load it. Just be sure the ProductId is uniqiue positive integer
.

The application is controlled in a menu system that is mainly based on integer input. It is possiple to

*  View all or some posts.
* Sort the view according to different attributes of the post.
* Add new expenses or incomes
* Edit the posts
* Remove Posts
* save and exit in a good way.

Previous entered posts are loaded upon starting up the application.
