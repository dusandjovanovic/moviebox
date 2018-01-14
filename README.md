# moviebox
Universal Windows Application for getting and storing movie and series metadata. Neo4j graph database integration.

MovieBox  je aplikacija koji omogućava organizovanje i lako održavanje kolekcije filmova. Održavanje kolekcije podrazumeva dodavanje, modifikaciju i brisanje filmova. Pored filmova postoji i pregled sezona serija. MovieBox omogućava nalaženje odredjenog filma iz kolekcije i reprodukciju sadržaja ukoliko je taj konkretan film prisutan na fajlsistemu korisnika. Pored održavanje biblioteke MovieBox pruža i pribavljanje preporučenih filmova, formiranih na osnovu trenutnog sadržaja biblioteke.

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


Atributi entiteta Movie:

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

Atributi entiteta TVShow:

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
