using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class SeedInitialData : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            var sqlScript =
            @"INSERT INTO ""Movies"" (""Id"", ""Title"", ""Director"", ""Producer"", ""ReleaseDate"", ""OpeningCrawl"", ""Characters"", ""CreatedAt"", ""DeletedAt"")
            VALUES
            (
                '890faaaf-04df-41bd-bed6-c9bbe13c0ced',
                'Jaws',
                'Steven Spielberg',
                'Richard D. Zanuck',
                '1975-06-20T00:00:00Z',
                'Jaws… beneath the coastal waves at night, a shark takes a quick but fatal meal out of a swimming woman. Can you still hear her screams or see the floating sea beacon she held onto? How about that whole struggle that seemed to last hours when in fact it was barely a minute long or so? Fear the deep people, fishing season comin'' right up.',
                '[""Chief Martin Brody"",""Matt Hooper"",""Quint"",""Ellen Brody"",""Mayor Larry Vaughn"",""Harry Meadows"",""Michael Brody"",""Deputy Hendricks"",""Mrs. Kintner"",""Ben Gardner""]'::jsonb,
                NOW(),
                NULL
            ),
            (
                'e44eb399-ac8e-43a7-a26b-d941daf48d6f',
                'Jurassic Park',
                'Steven Spielberg',
                'Kathleen Kennedy, Gerald R. Molen',
                '1993-06-11T00:00:00Z',
                'During a preview tour of a groundbreaking theme park, cloned dinosaurs break free and run havoc. Scientists and visitors must fight for survival amid prehistoric predators.',
                '[""Dr. Alan Grant"",""Dr. Ellie Sattler"",""Dr. Ian Malcolm"",""John Hammond"",""Tim Murphy"",""Lex Murphy"",""Robert Muldoon"",""Dennis Nedry"",""Ray Arnold"",""Donald Gennaro""]'::jsonb,
                NOW(),
                NULL
            ),
            (
                '931190d2-1927-4dd4-92db-ac6259956683',
                'E.T. the Extra-Terrestrial',
                'Steven Spielberg',
                'Kathleen Kennedy, Steven Spielberg',
                '1982-06-11T00:00:00Z',
                'A gentle alien becomes stranded on Earth and befriends a young boy. Together, they embark on a heartwarming adventure to help E.T. phone home.',
                '[""Elliott"",""ET"",""Michael"",""Gertie"",""Mary"",""Keys"",""Greg"",""Steve"",""Gregs Friend"",""Government Agent""]'::jsonb,
                NOW(),
                NULL
            ),
            (
                'a1d6a44b-05b9-48a5-bbca-006e80fa2507',
                'Star Wars: A New Hope',
                'George Lucas',
                'Gary Kurtz, Rick McCallum',
                '1977-05-25T00:00:00Z',
                'It is a period of civil war. Rebel spaceships, striking from a hidden base, have won their first victory against the evil Galactic Empire.…',
                '[""Luke Skywalker"",""Princess Leia"",""Han Solo"",""Obi-Wan Kenobi"",""Darth Vader"",""Chewbacca"",""C-3PO"",""R2-D2"",""Grand Moff Tarkin"",""Biggs Darklighter""]'::jsonb,
                NOW(),
                NULL
            ),
            (
                '417a5af0-5168-48c1-89c7-1fdf2470fa1e',
                'The Matrix',
                'Lana Wachowski, Lilly Wachowski',
                'Joel Silver',
                '1999-03-31T00:00:00Z',
                'A computer hacker discovers the shocking truth: reality is a simulated world controlled by machines. He joins a rebellion to free humanity.',
                '[""Neo"",""Morpheus"",""Trinity"",""Agent Smith"",""Cypher"",""Tank"",""Dozer"",""The Oracle"",""Niobe"",""Mouse""]'::jsonb,
                NOW(),
                NULL
            ),
            (
                '4d4a75e3-7cad-47e8-8d64-c24e79305520',
                'Inception',
                'Christopher Nolan',
                'Emma Thomas, Christopher Nolan',
                '2010-07-16T00:00:00Z',
                'A skilled thief steals corporate secrets through dream-sharing technology. He is tasked with planting an idea, but deeper layers of the subconscious blur the line between dream and reality.',
                '[""Dom Cobb"",""Arthur"",""Ariadne"",""Eames"",""Robert Fischer"",""Mal"",""Saito"",""Yusuf"",""Nash"",""Browning""]'::jsonb,
                NOW(),
                NULL
            ),
            (
                '54fe71dd-7f5e-47c4-a75b-ac8f14dee662',
                'Titanic',
                'James Cameron',
                'James Cameron, Jon Landau',
                '1997-12-19T00:00:00Z',
                'A young aristocrat falls in love with a poor artist aboard the ill-fated RMS Titanic. Their romance is tested as the ship meets its tragic end.',
                '[""Jack Dawson"",""Rose DeWitt Bukater"",""Caledon Hockley"",""Ruth DeWitt Bukater"",""Molly Brown"",""Captain Smith"",""Thomas Andrews"",""Spicer Lovejoy"",""Fabrizio"",""Tommy Ryan""]'::jsonb,
                NOW(),
                NULL
            ),
            (
                '82c754d3-0f68-48c7-a62d-e11897b73115',
                'Avatar',
                'James Cameron',
                'James Cameron, Jon Landau',
                '2009-12-18T00:00:00Z',
                'On the lush alien world of Pandora, a paraplegic Marine is torn between following orders and protecting the world he feels is his home.',
                '[""Jake Sully"",""Neytiri"",""Dr. Grace Augustine"",""Colonel Miles Quaritch"",""Trudy Chacon"",""Norm Spellman"",""Moat"",""Tsutey"",""Eytukan"",""Parker Selfridge""]'::jsonb,
                NOW(),
                NULL
            ),
            (
                '3b2c1f4e-0d5a-4f8b-9c7d-6a2e5f1b8c3e',
                'The Godfather',
                'Francis Ford Coppola',
                'Albert S. Ruddy',
                '1972-03-24T00:00:00Z',
                'The aging patriarch of an organized crime dynasty transfers control of his clandestine empire to his reluctant son.',
                '[""Vito Corleone"",""Michael Corleone"",""Sonny Corleone"",""Tom Hagen"",""Kay Adams"",""Fredo Corleone"",""Connie Corleone"",""Clemenza"",""Tessio"",""Carlo Rizzi""]'::jsonb,
                NOW(),
                NULL
            ),
            (
                '5f3b8c2d-0a1e-4b6f-8c7d-9a2e5f1b8c3e',
                'Back to the Future',
                'Robert Zemeckis',
                'Bob Gale, Neil Canton',
                '1985-07-03T00:00:00Z',
                'Marty McFly accidentally travels back in time 30 years and must ensure his teenage parents fall in love or he’ll cease to exist.',
                '[""Marty McFly"",""Dr. Emmett Brown"",""Lorraine Baines"",""George McFly"",""Biff Tannen"",""Jennifer Parker"",""Strickland"",""Goldie Wilson"",""Marvin Berry"",""Needles""]'::jsonb,
                NOW(),
                NULL
            );

            INSERT INTO ""Roles"" (""Id"", ""Name"", ""Description"", ""CreatedAt"", ""DeletedAt"")
            VALUES
                ('1894b6b5-9022-481f-8201-3750be0bcbdb', 'Master', 'Jedi Master role', NOW(), NULL),
                ('35912b99-310b-417d-8a99-f88a8dc015ea', 'Padawan', 'Jedi Apprentice role', NOW(), NULL),
                ('a2478132-4100-4f48-9a79-391a239363d9', 'LordSith', 'Sith Lord role', NOW(), NULL);

            INSERT INTO ""Users"" (""Id"", ""Email"", ""PasswordHash"", ""RoleId"", ""CreatedAt"", ""DeletedAt"")
            VALUES
                ('b82dae41-e453-417b-9827-91d2c13c7d32', 'anakin@jedi.com', 'AQAAAAIAAYagAAAAEG60IT9thr4/vhSW8Fx2+vDiBT6pyLnUiiJ8wN/Wqxo3HeN9QpLdSOs8tFqrOyVp7Q==', '35912b99-310b-417d-8a99-f88a8dc015ea', NOW(), NULL),
                ('2349edf0-f48c-44b8-bead-478578d6e2c5', 'obiwan@jedi.com', 'AQAAAAIAAYagAAAAEA+QOCjRpr4snr4fCQRMGgRbr8qAKY37hsWEh3DgPiThunsOM6LfhuQZjXKlvgTiNw==', '1894b6b5-9022-481f-8201-3750be0bcbdb', NOW(), NULL)";

            migrationBuilder.Sql(sqlScript);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            //nullify
        }
    }
}
