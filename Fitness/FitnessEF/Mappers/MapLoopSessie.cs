using System;
using System.Collections.Generic;
using System.Linq;
using FitnessBL.Model; // Domein model
using FitnessEF.Mappers;
using FitnessEF.Model; // Entity Framework model

namespace FitnessBL.Mappers
{
    public static class MapLoopSessie
    {
        /*
        // Mapper van EF naar Domein
        public static LoopSessie MapToDomain(LoopSessieEF loopSessieEF)
        {
            if (loopSessieEF == null)
            {
                throw new ArgumentNullException(
                    nameof(loopSessieEF),
                    "De EF entiteit kan niet null zijn."
                );
            }
            
             
            klant moet nog gevonden worden => loopSessieEF heeft enkel klanten ID
             
            Klant klant = MapKlant.MapToDomain(loopSessieEF.Klant);
            return new LoopSessie(
                loopSessieEF.Datum,
                loopSessieEF.Duur,
                loopSessieEF.GemiddeldeSnelheid,
                klant
            );
        }
             

        // Mapper van Domein naar EF
        public static LoopSessieEF MapToEF(LoopSessie domainModel)
        {
            if (domainModel == null)
            {
                throw new ArgumentNullException(
                    nameof(domainModel),
                    "Het domein model kan niet null zijn."
                );
            }

            return new LoopSessieEF
            {
                Datum = domainModel.Datum,
                Duur = domainModel.Duur,
                GemiddeldeSnelheid = domainModel.GemiddeldeSnelheid,
                MemberId = domainModel.Klant.Id
            };
        }

        // Mapper voor een lijst van EF naar Domein
        public static List<LoopSessie> MapToDomainList(List<LoopSessieEF> loopSessiesEF)
        {
            if (loopSessiesEF == null)
            {
                throw new ArgumentNullException(
                    nameof(loopSessiesEF),
                    "De EF entiteitenlijst kan niet null zijn."
                );
            }

            return loopSessiesEF.Select(LoopSessieEF => MapToDomain(LoopSessieEF)).ToList();
        }

        // Mapper voor een lijst van Domein naar EF
        public static List<LoopSessieEF> MapToEFList(List<LoopSessie> domainModels)
        {
            if (domainModels == null)
            {
                throw new ArgumentNullException(
                    nameof(domainModels),
                    "De domein modellenlijst kan niet null zijn."
                );
            }

            return domainModels.Select(domainModel => MapToEF(domainModel)).ToList();
        }
         */
    }
}
