using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using FitnessAPI.DTO;
using FitnessBL.Model;
using FitnessWPF.Excepitons;
using Newtonsoft.Json;

namespace FitnessWPF
{
    public class FitnessClient
    {
        static HttpClient client = new HttpClient();

        public FitnessClient()
        {
            client.BaseAddress = new Uri("https://localhost:7240/");

            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(
                new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json")
            );
        }

        public async Task<List<Member>> GetMember(string voornaam, string achternaam)
        {
            try
            {
                HttpResponseMessage response = await client.GetAsync(
                    $"/MemberViaNaam/{voornaam}/{achternaam}"
                );
                response.EnsureSuccessStatusCode();

                string responseBody = await response.Content.ReadAsStringAsync();
                List<Member>? members = JsonConvert.DeserializeObject<List<Member>>(responseBody);

                return members;
            }
            catch (Exception ex)
            {
                throw new FitnessClientException($"Error on retrieving members: {ex.Message}", ex);
            }
        }

        public async Task<List<Reservation>> GetReservationsMember(Member member)
        {
            try
            {
                int id = member.Member_id;
                HttpResponseMessage response = await client.GetAsync($"/ReservationsMember/{id}");
                response.EnsureSuccessStatusCode();

                string responseBody = await response.Content.ReadAsStringAsync();
                List<Reservation>? reservations = JsonConvert.DeserializeObject<List<Reservation>>(
                    responseBody
                );

                return reservations;
            }
            catch (Exception ex)
            {
                throw new FitnessClientException(
                    $"Error on retrieving reservations that a member made: {ex.Message}",
                    ex
                );
            }
        }

        public async Task<List<Time_slot>> GetTimeSlots()
        {
            try
            {
                HttpResponseMessage response = await client.GetAsync($"/LijstTimeSlots");
                response.EnsureSuccessStatusCode();

                string responseBody = await response.Content.ReadAsStringAsync();
                List<Time_slot>? timeSlots = JsonConvert.DeserializeObject<List<Time_slot>>(
                    responseBody
                );

                return timeSlots;
            }
            catch (Exception ex)
            {
                throw new FitnessClientException(
                    $"Error on retrieving time slots: {ex.Message}",
                    ex
                );
            }
        }

        public async Task<Time_slot> GetTimeSlotId(int id)
        {
            try
            {
                HttpResponseMessage response = await client.GetAsync($"/Time_slotViaId/{id}");
                response.EnsureSuccessStatusCode();

                string responseBody = await response.Content.ReadAsStringAsync();
                Time_slot? timeslot = JsonConvert.DeserializeObject<Time_slot>(responseBody);

                return timeslot;
            }
            catch (Exception ex)
            {
                throw new FitnessClientException(
                    $"Error on retrieving the timeslot that correspondend with the id: {ex.Message}",
                    ex
                );
            }
        }

        public async Task<List<Equipment>> GetAvailableEquipment(DateTime date, int TimeSlotId)
        {
            try
            {
                HttpResponseMessage response = await client.GetAsync(
                    $"/AllAvailableEquipment/{date}/{TimeSlotId}"
                );
                response.EnsureSuccessStatusCode();

                string responseBody = await response.Content.ReadAsStringAsync();
                List<Equipment> equipments = JsonConvert.DeserializeObject<List<Equipment>>(
                    responseBody
                );

                return equipments;
            }
            catch (Exception ex)
            {
                throw new FitnessClientException(
                    $"Error on retrieving available equipments: {ex.Message}",
                    ex
                );
            }
        }

        public async Task<string> CreateReservation(ReservationAanmakenDTO reservation)
        {
            try
            {
                string json = JsonConvert.SerializeObject(reservation);
                StringContent content = new StringContent(json, Encoding.UTF8, "application/json");

                HttpResponseMessage response = await client.PostAsync(
                    "/ReservationAanmaken",
                    content
                );

                if (response.IsSuccessStatusCode)
                {
                    return null; // Geen foutmelding bij succes
                }
                else
                {
                    // Lees de foutmelding uit de response body
                    string error = await response.Content.ReadAsStringAsync();
                    return error;
                }
            }
            catch (Exception ex)
            {
                throw new FitnessClientException(
                    $"Error on creating the new reservation: {ex.Message}",
                    ex
                );
            }
        }
    }
}
