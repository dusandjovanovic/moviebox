# moviebox
Universal Windows Application for getting and storing movie and series metadata. Neo4j graph database integration.

MovieBox  je aplikacija koji omogućava organizovanje i lako održavanje kolekcije filmova. Održavanje kolekcije podrazumeva dodavanje, modifikaciju i brisanje filmova. Pored filmova postoji i pregled sezona serija. MovieBox omogućava nalaženje odredjenog filma iz kolekcije i reprodukciju sadržaja ukoliko je taj konkretan film prisutan na fajlsistemu korisnika. Pored održavanje biblioteke MovieBox pruža i pribavljanje preporučenih filmova, formiranih na osnovu trenutnog sadržaja biblioteke.


![alt text][main]

[main]: metadata/movies.PNG


### Graph baza Neo4j
Za održavanje perzistencije koristi se neo4j baza podataka u koju se preslikava model podataka prisutan u samoj logici aplikacije.
Neophodno je pokrenuti neo4j runtime iz foldera **neo4j/bin/** komandom `neo4j console`. Ne treba zatvarati konekciju prilikom korišćenja aplikacije.

`username: neo4j`

`password: admin`

Za pristupanje bazi podataka koristi se [neo4jclient](https://github.com/Readify/Neo4jClient).

#### Struktuiranost graph baze

`Nodes: Movie, Genre, Actor, Director, Writer`

`(Movie)-[IS_GENRE]->(Genre)`

`(Movie)<-[ACTS_IN]-(Actor)`

`(Movie)<-[DIRECTED]-(Director)`

`(Movie)<-[WROTE]-(Writer)`


Atributi entiteta Movie (primer):

`{
  "Path": "",
  "Popularity": 66.554448,
  "VoteAverage": 8.3,
  "Title": "Fight Club",
  "Overview": "A ticking-time-bomb insomniac and a slippery soap salesman channel primal male aggression into a shocking new form of therapy. Their concept catches on, with underground "fight clubs" forming in every town, until an eccentric gets in the way and ignites an out-of-control spiral toward oblivion.",
  "ReleaseDate": 939938400,
  "IsAdultThemed": false,
  "BackdropPath": "/87hTDiay2N2qWyX4Ds7ybXi9h8I.jpg",
  "Runtime": 139,
  "Year": 1999,
  "VoteCount": 11009,
  "OriginalTitle": "Fight Club",
  "Poster": "C:\Users\dusan\AppData\Local\Packages\62b1cd77-cc72-459c-bdbc-849043f241cb_p29g5cerja8bg\LocalState\Fight Club.jpg",
  "Id": 550
}`

![alt text][movieGraph]

[movieGraph]: metadata/graph.png

`Nodes: TVShow, Genre, TVShowCreator, Season, Network`

`(TVShow)-[IS_GENRE]->(Genre)`

`(TVShow)<-[CREATED]-(TVShowCreator)`

`(TVShow)-[BELONGS]->(Network)`

`(TVShow)-[HAS]->(Season)`

Atributi entiteta TVShow (primer):

`{
  "Path": "",
  "OriginalLanguage": "en",
  "Homepage": "http://www.hbo.com/game-of-thrones",
  "NumberOfSeasons": 0,
  "EpisodeRunTime": 60,
  "LastAirDate": 1567029600,
  "InProduction": true,
  "Popularity": 81.243277,
  "Overview": "Seven noble families fight for control of the mythical land of Westeros. Friction between the houses leads to full-scale war. All while a very ancient evil awakens in the farthest north. Amidst the war, a neglected military order of misfits, the Night's Watch, is all that stands between the realms of men and icy horrors beyond.",
  "FirstAirDate": 1302991200,
  "OriginalName": "Game of Thrones",
  "PosterPath": "C:\Users\dusan\AppData\Local\Packages\62b1cd77-cc72-459c-bdbc-849043f241cb_p29g5cerja8bg\LocalState\Game of Thrones Season 4.jpg",
  "Name": "Game of Thrones",
  "BackdropPath": "/gX8SYlnL9ZznfZwEH4KJUePBFUM.jpg",
  "NumberOfEpisodes": 0,
  "Id": 1399
}`

![alt text][seriesGraph]

[seriesGraph]: metadata/graph(1).png


#### Pristupanje bazi podataka

Pristupanje bazi podataka ostvaruje se kroz statičku klasu NeoSingleton u kojoj se inicijalizuje instanca konekcije sa bazom.

`if (_instance == null)
                    _instance = new GraphClient(new Uri("http://localhost:7474/db/data"), "neo4j", "admin");`

Metode:

`_addMovieAsync(Movie movie)` za dodavanje novog entiteta filma u bazu podataka, kao i dodavanje svih elemenata sa kojima je u vezi.

`_removeMovieAsync(Movie todelete)` za brisanje postojećeg filma i svih veza.

`_modifyMovie(Movie tomodify)` za promenu atributa postojećeg filma.

`_allActors()` za dobijanje liste svih glumaca prisutnih u bazi.

`_allGenres()` za dobijanje liste svih žanrova prisutnih u bazi.

`_allDirectors()` za dobijanje liste svih režisera prisutnih u bazi.

`_allWriters()` za dobijanje liste svih pisaca prisutnih u bazi.

`_readByMultiple(int runtimeLow, int runtimeHigh, int yearLow, int yearHigh, IEnumerable<object> actors, IEnumerable<object> directors, IEnumerable<object> genres)` za dobijanje liste filmova koja odgovara atributima metode (biće objašnjeno kasnije).

`_readByMeta(object actorName, object directorName, object genreName)` za filtriranje filmova po glumcu, režiseru i žanru.

`_readAll()` za dobijanje svih filmova iz baze, kao elemenata koji su u vezi sa njima.


`_readTVShows()` za dovijanje svih serija iz baze, kao elemenata koji su u vezi sa njima.

`_addTvShow(TVShow show)` za dodavanje novog entiteta serije.

`_addSeason(Season newSeason, TVShow show)` za dodavanje sezone seriji koja je već prisutna u bazi.

`_removeTVShow(TVShow todelete)` za brisanje celokupne serije i svih elemenata sa kojima je u vezi.

`_removeSeason(Season todelete)` za brisanje sezone postojeće serije.


Primer Cypher naredbi i pristupanja bazi na metodi `ObservableCollection<Movie> _readByMultiple`
Atributi:
* `int runtimeLow` : donja granica trajanja

* `int runtimeHigh` : gornja granica trajanja

* `int yearLow` : donja granica godine koja je veza

* `int yearHigh` : gornja granica godine

* `IEnumerable<object> actors` : lista glumaca

* `IEnumerable<object> directors` : lista režisera

* `IEnumerable<object> genres` : lista žanrova

            `var returned = _instance.Cypher
               .Match("(movie:Movie)<-[ACTS_IN]-(actor:Actor)")
               .Match("(movie:Movie)<-[DIRECTED]-(director:Director)")
               .Match("(movie:Movie)-[IS_GENRE]->(genre:Genre)")
               .Where((NeoMovie movie) => movie.Runtime >= runtimeLow)
               .AndWhere((NeoMovie movie) => movie.Runtime <= runtimeHigh)
               .AndWhere((NeoMovie movie) => movie.Year >= yearLow)
               .AndWhere((NeoMovie movie) => movie.Year <= yearHigh)
               .AndWhere("actor.Name in {actorsList}")
               .AndWhere("genre.Name in {genresList}")
               .AndWhere("director.Name in {directorsList}")
               .WithParams(new
               {
                   actorsList,
                   genresList,
                   directorsList
               })
               .ReturnDistinct(movie => movie.As<NeoMovie>())
               .Results;`
       
       
![alt text][detailed]

[detailed]: metadata/detailed.PNG
