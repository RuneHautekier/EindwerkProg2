using FitnessAPI.DTO;
using FitnessBL.Enums;
using FitnessBL.Exceptions;
using FitnessBL.Model;
using FitnessBL.Services;
using FitnessEF.Model;
using Microsoft.AspNetCore.Mvc;
using ProgramBL = FitnessBL.Model.Program;

namespace FitnessAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MemberController : ControllerBase
    {
        private MemberService memberService;

        public MemberController(MemberService memberService)
        {
            this.memberService = memberService;
        }

        [HttpGet("/LijstMembers")]
        public ActionResult<IEnumerable<Member>> GetMembers()
        {
            try
            {
                IEnumerable<Member> members = memberService.GetMembers();
                return Ok(members);
            }
            catch (ServiceException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("/GetTrainingSessionsMember/{id}")]
        public IActionResult GetTrainingSessionsMember(int id)
        {
            try
            {
                Member member = memberService.GetMemberId(id);
                IEnumerable<TrainingSession> TrainingSessions =
                    memberService.GetTrainingSessionsMember(member);

                // Maak een lijst om dynamische DTO-objecten op te slaan
                List<dynamic> trainingSessionDTOs = new List<dynamic>();

                // Itereer door alle trainingssessies
                foreach (TrainingSession ts in TrainingSessions)
                {
                    // Controleer of het een RunningSession is
                    if (ts is Runningsession_main runningSession)
                    {
                        // Voeg een DTO-object toe met alleen relevante velden voor een RunningSession
                        trainingSessionDTOs.Add(
                            new
                            {
                                SessionType = "Running",
                                Id = runningSession.Runningsession_id,
                                Date = runningSession.Date,
                                Duration = runningSession.Duration,
                                AvgSpeed = runningSession.Avg_speed
                            }
                        );
                    }
                    // Controleer of het een CyclingSession is
                    else if (ts is Cyclingsession cyclingSession)
                    {
                        // Voeg een DTO-object toe met alleen relevante velden voor een CyclingSession
                        trainingSessionDTOs.Add(
                            new
                            {
                                SessionType = "Cycling",
                                Id = cyclingSession.Cyclingsession_id,
                                Date = cyclingSession.Date,
                                Duration = cyclingSession.Duration,
                                AvgWatt = cyclingSession.Avg_watt,
                                MaxWatt = cyclingSession.Max_watt,
                                AvgCadence = cyclingSession.Avg_cadence,
                                MaxCadence = cyclingSession.Max_cadence,
                                TrainingsType = cyclingSession.TrainingsType,
                                Comment = cyclingSession.Comment
                            }
                        );
                    }
                }

                return Ok(trainingSessionDTOs);
            }
            catch (ServiceException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("/ProgramsMember/{id}")]
        public IActionResult GetProgramListMember(int id)
        {
            try
            {
                Member member = memberService.GetMemberId(id);
                IEnumerable<ProgramBL> programs = memberService.GetProgramListMember(member);

                List<ProgramDTO> programDTOs = new List<ProgramDTO>();
                foreach (ProgramBL program in programs)
                {
                    ProgramDTO pDTO = new ProgramDTO
                    {
                        ProgramCode = program.ProgramCode,
                        Name = program.Name,
                        Target = program.Target,
                        StartDate = program.Startdate.Date,
                        MaxMembers = program.Max_members
                    };

                    programDTOs.Add(pDTO);
                }

                return Ok(programDTOs);
            }
            catch (ServiceException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("/GetTrainingSessionsMemberVanMaandInJaar/{id}/{maand}/{jaar}")]
        public IActionResult TrainingSessionsMemberPerMaandInJaar(int id, int maand, int jaar)
        {
            try
            {
                DateTime date = new DateTime(jaar, maand, 1);
                Member member = memberService.GetMemberId(id);
                IEnumerable<TrainingSession> TrainingSessions =
                    memberService.GetTrainingSessionsMemberInMaandInJaar(member, date);

                // Maak een lijst om dynamische DTO-objecten op te slaan
                List<dynamic> trainingSessionDTOs = new List<dynamic>();

                // Itereer door alle trainingssessies
                foreach (TrainingSession ts in TrainingSessions)
                {
                    // Controleer of het een RunningSession is
                    if (ts is Runningsession_main runningSession)
                    {
                        // Voeg een DTO-object toe met alleen relevante velden voor een RunningSession
                        trainingSessionDTOs.Add(
                            new
                            {
                                SessionType = "Running",
                                Id = runningSession.Runningsession_id,
                                Date = runningSession.Date,
                                Duration = runningSession.Duration,
                                AvgSpeed = runningSession.Avg_speed
                            }
                        );
                    }
                    // Controleer of het een CyclingSession is
                    else if (ts is Cyclingsession cyclingSession)
                    {
                        // Voeg een DTO-object toe met alleen relevante velden voor een CyclingSession
                        trainingSessionDTOs.Add(
                            new
                            {
                                SessionType = "Cycling",
                                Id = cyclingSession.Cyclingsession_id,
                                Date = cyclingSession.Date,
                                Duration = cyclingSession.Duration,
                                AvgWatt = cyclingSession.Avg_watt,
                                MaxWatt = cyclingSession.Max_watt,
                                AvgCadence = cyclingSession.Avg_cadence,
                                MaxCadence = cyclingSession.Max_cadence,
                                TrainingsType = cyclingSession.TrainingsType,
                                Comment = cyclingSession.Comment
                            }
                        );
                    }
                }

                return Ok(trainingSessionDTOs);
            }
            catch (ServiceException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("/GetTrainingSessionsMemberGegevensTijd/{id}")]
        public IActionResult GetTrainingSessionsMemberGegevensTijd(int id)
        {
            try
            {
                Member member = memberService.GetMemberId(id);
                IEnumerable<TrainingSession> TrainingSessions =
                    memberService.GetTrainingSessionsMember(member);

                TrainingSession langsteTs = TrainingSessions.First();
                TrainingSession kortsteTs = TrainingSessions.First();
                decimal aantalMinuten = 0;

                foreach (TrainingSession trainingSession in TrainingSessions)
                {
                    aantalMinuten += trainingSession.Duration;
                    if (langsteTs.Duration > trainingSession.Duration)
                        langsteTs = trainingSession;
                    if (kortsteTs.Duration < trainingSession.Duration)
                        kortsteTs = trainingSession;
                }

                TrainingSessionDTO langsteTsDTO = null;
                TrainingSessionDTO kortsteTsDTO = null;

                if (langsteTs is Runningsession_main runningSession)
                {
                    // Voeg een DTO-object toe met alleen relevante velden voor een RunningSession
                    langsteTsDTO = new TrainingSessionDTO
                    {
                        SessionType = "Running",
                        Id = runningSession.Runningsession_id,
                        Date = runningSession.Date,
                        Duration = runningSession.Duration,
                        AvgSpeed = runningSession.Avg_speed
                    };
                }
                else if (langsteTs is Cyclingsession cyclingSession)
                {
                    // Voeg een DTO-object toe met alleen relevante velden voor een CyclingSession
                    langsteTsDTO = new TrainingSessionDTO
                    {
                        SessionType = "Cycling",
                        Id = cyclingSession.Cyclingsession_id,
                        Date = cyclingSession.Date,
                        Duration = cyclingSession.Duration,
                        AvgWatt = cyclingSession.Avg_watt,
                        MaxWatt = cyclingSession.Max_watt,
                        AvgCadence = cyclingSession.Avg_cadence,
                        MaxCadence = cyclingSession.Max_cadence,
                        TrainingsType = cyclingSession.TrainingsType,
                        Comment = cyclingSession.Comment
                    };
                }

                if (kortsteTs is Runningsession_main runningSession2)
                {
                    // Voeg een DTO-object toe met alleen relevante velden voor een RunningSession
                    kortsteTsDTO = new TrainingSessionDTO
                    {
                        SessionType = "Running",
                        Id = runningSession2.Runningsession_id,
                        Date = runningSession2.Date,
                        Duration = runningSession2.Duration,
                        AvgSpeed = runningSession2.Avg_speed
                    };
                }
                else if (kortsteTs is Cyclingsession cyclingSession)
                {
                    // Voeg een DTO-object toe met alleen relevante velden voor een CyclingSession
                    kortsteTsDTO = new TrainingSessionDTO
                    {
                        SessionType = "Cycling",
                        Id = cyclingSession.Cyclingsession_id,
                        Date = cyclingSession.Date,
                        Duration = cyclingSession.Duration,
                        AvgWatt = cyclingSession.Avg_watt,
                        MaxWatt = cyclingSession.Max_watt,
                        AvgCadence = cyclingSession.Avg_cadence,
                        MaxCadence = cyclingSession.Max_cadence,
                        TrainingsType = cyclingSession.TrainingsType,
                        Comment = cyclingSession.Comment
                    };
                }

                decimal aantalUren = aantalMinuten / 60;

                TrainingSessionMetLangsteEnKortsteDTO tsDTO =
                    new TrainingSessionMetLangsteEnKortsteDTO
                    {
                        AantalTrainingSessions = TrainingSessions.Count(),
                        AantalUren = Math.Round(aantalUren, 2),
                        LangsteTrainingSession = langsteTsDTO,
                        KortsteTrainingSession = kortsteTsDTO,
                        GemiddeldeDuur = Math.Round(aantalMinuten / TrainingSessions.Count()),
                    };

                return Ok(tsDTO);
            }
            catch (ServiceException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("/GetTrainingSessionsMemberInMaandInJaar/{id}/{maand}/{jaar}")]
        public IActionResult GetTrainingSessionsMemberInMaandInJaar(int id, int maand, int jaar)
        {
            try
            {
                DateTime date = new DateTime(jaar, maand, 1);
                Member member = memberService.GetMemberId(id);
                IEnumerable<TrainingSession> TrainingSessions =
                    memberService.GetTrainingSessionsMemberInMaandInJaar(member, date);

                // Maak een lijst om dynamische DTO-objecten op te slaan
                List<dynamic> trainingSessionDTOs = new List<dynamic>();

                // Itereer door alle trainingssessies
                foreach (TrainingSession ts in TrainingSessions)
                {
                    // Controleer of het een RunningSession is
                    if (ts is Runningsession_main runningSession)
                    {
                        // Voeg een DTO-object toe met alleen relevante velden voor een RunningSession
                        trainingSessionDTOs.Add(
                            new
                            {
                                SessionType = "Running",
                                Id = runningSession.Runningsession_id,
                                Date = runningSession.Date,
                                Duration = runningSession.Duration,
                                AvgSpeed = runningSession.Avg_speed
                            }
                        );
                    }
                    // Controleer of het een CyclingSession is
                    else if (ts is Cyclingsession cyclingSession)
                    {
                        string trainingsImpact = "High";

                        if (cyclingSession.Avg_watt < 150 && cyclingSession.Duration <= 90)
                            trainingsImpact = "Low";
                        if (
                            cyclingSession.Avg_watt < 150 && cyclingSession.Duration > 90
                            || cyclingSession.Avg_watt >= 150 && cyclingSession.Avg_watt <= 200
                        )
                            trainingsImpact = "Medium";

                        // Voeg een DTO-object toe met alleen relevante velden voor een CyclingSession
                        trainingSessionDTOs.Add(
                            new
                            {
                                SessionType = "Cycling",
                                Id = cyclingSession.Cyclingsession_id,
                                Date = cyclingSession.Date,
                                Duration = cyclingSession.Duration,
                                AvgWatt = cyclingSession.Avg_watt,
                                MaxWatt = cyclingSession.Max_watt,
                                AvgCadence = cyclingSession.Avg_cadence,
                                MaxCadence = cyclingSession.Max_cadence,
                                TrainingsType = cyclingSession.TrainingsType,
                                Comment = cyclingSession.Comment,
                                TrainingsImpact = trainingsImpact
                            }
                        );
                    }
                }

                return Ok(trainingSessionDTOs);
            }
            catch (ServiceException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("/GetTrainingSessionsMemberAantalPerMaandInJaar/{id}/{jaar}")]
        public IActionResult GetTrainingSessionsMemberAantalPerMaandInJaar(int id, int jaar)
        {
            try
            {
                DateTime date = new DateTime(jaar, 1, 1);
                Member member = memberService.GetMemberId(id);
                Dictionary<int, int> sessiesPerMaand =
                    memberService.GetTrainingSessionsMemberAantalPerMaandInJaar(member, date);
                Dictionary<string, string> result = new Dictionary<string, string>();

                foreach (KeyValuePair<int, int> kvp in sessiesPerMaand)
                {
                    result.Add($"Maand {kvp.Key}", $"Aantal Sessies: {kvp.Value}");
                }

                TrainingSessionMemberSessiesAantalPerMaandDTO aantalPerMaandDTO =
                    new TrainingSessionMemberSessiesAantalPerMaandDTO { SessiesPerMaand = result };

                return Ok(aantalPerMaandDTO);
            }
            catch (ServiceException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("/GetTrainingSessionsMemberAantalPerMaandInJaarMetType/{id}/{jaar}")]
        public IActionResult GetTrainingSessionsMemberAantalPerMaandInJaarMetType(int id, int jaar)
        {
            try
            {
                DateTime date = new DateTime(jaar, 1, 1);
                Member member = memberService.GetMemberId(id);
                Dictionary<string, Dictionary<int, int>> sessiesPerMaand =
                    memberService.GetTrainingSessionsMemberAantalPerMaandInJaarMetType(
                        member,
                        date
                    );
                Dictionary<string, string> tussenResultaat = new Dictionary<string, string>();

                Dictionary<string, Dictionary<string, string>> result =
                    new Dictionary<string, Dictionary<string, string>>();

                foreach (KeyValuePair<string, Dictionary<int, int>> kvp in sessiesPerMaand)
                {
                    foreach (KeyValuePair<int, int> keyValuePair in kvp.Value)
                    {
                        tussenResultaat.Add(
                            $"Maand {keyValuePair.Key}",
                            $"Aantal Sessies: {keyValuePair.Value}"
                        );
                    }

                    result.Add(kvp.Key, tussenResultaat);
                    tussenResultaat = new Dictionary<string, string>();
                }

                TrainingSessionMemberAantalPerMaandInJaarMetTypeDTO aantalPerMaandDTO =
                    new TrainingSessionMemberAantalPerMaandInJaarMetTypeDTO
                    {
                        SessiesPerMaand = result
                    };

                return Ok(aantalPerMaandDTO);
            }
            catch (ServiceException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("/MemberViaId/{id}")]
        public IActionResult GetMemberId(int id)
        {
            try
            {
                Member member = memberService.GetMemberId(id);
                return Ok(member);
            }
            catch (ServiceException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("/MemberToevoegen")]
        public IActionResult AddMember([FromBody] MemberAanmakenDTO memberDTO)
        {
            try
            {
                List<string> intr = new List<string>();
                if (memberDTO.Intrests.Count >= 1 && !memberDTO.Intrests.Contains("string"))
                {
                    foreach (string str in memberDTO.Intrests)
                    {
                        intr.Add(str);
                    }
                }
                Member member = new Member(
                    memberDTO.FirstName,
                    memberDTO.LastName,
                    memberDTO.Email,
                    memberDTO.Address,
                    memberDTO.Birthday,
                    intr,
                    memberDTO.TypeMember
                );

                memberService.AddMember(member);

                return CreatedAtAction(
                    nameof(GetMemberId), // Specify the action name of the "Get" endpoint
                    new { id = member.Member_id }, // Parameter voor de Get eindpoint
                    member // Return the created gebruiker object
                );
            }
            catch (MemberException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (ServiceException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPatch("/MemberAanpassen/{id}")]
        public IActionResult UpdateMember(
            int id,
            [FromQuery] DateTime GeboorteDatum,
            [FromQuery] TypeKlant? typeKlant,
            [FromQuery] string? Voornaam = null,
            [FromQuery] string? Achternaam = null,
            [FromQuery] string? Email = null,
            [FromQuery] string? Adres = null,
            [FromQuery] List<string> Interesses = null
        )
        {
            try
            {
                // Haal de gebruiker op uit de database
                Member member = memberService.GetMemberId(id);

                // Pas alleen de velden aan die zijn meegegeven (niet null)
                if (!string.IsNullOrEmpty(Voornaam))
                    member.FirstName = Voornaam;
                if (!string.IsNullOrEmpty(Achternaam))
                    member.LastName = Achternaam;
                if (!string.IsNullOrEmpty(Email))
                    member.Email = Email;
                if (!string.IsNullOrEmpty(Adres))
                    member.Address = Adres;
                if (GeboorteDatum != new DateTime(0001, 01, 01))
                    member.Birthday = GeboorteDatum;
                if (typeKlant.HasValue)
                    member.MemberType = typeKlant.Value;
                if (Interesses != null && Interesses.Count() != 0)
                    member.Interests = Interesses;

                // Update het record in de database
                memberService.UpdateMember(member);

                return CreatedAtAction(
                    nameof(GetMemberId), // Specify the action name of the "Get" endpoint
                    new { id = member.Member_id }, // Parameter voor de Get eindpoint
                    member // Return the created gebruiker object
                );
            }
            catch (ServiceException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpDelete("/MemberVerwijderenViaId/{id}")]
        public IActionResult DeleteMember(int id)
        {
            try
            {
                Member member = memberService.GetMemberId(id);
                memberService.DeleteMember(member);
                return Ok("De member is succesvol verwijderd!");
            }
            catch (ServiceException ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
