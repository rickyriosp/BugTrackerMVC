﻿using BugTracker.Data;
using BugTracker.Models;
using BugTracker.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace BugTracker.Services
{
    public class BTTicketHistoryService : IBTTicketHistoryService
    {
        private readonly ApplicationDbContext _context;

        public BTTicketHistoryService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task AddHistoryAsync(Ticket oldTicket, Ticket newTicket, string userId)
        {
            // New Ticket has been added
            if (oldTicket == null && newTicket != null)
            {
                TicketHistory history = new()
                {
                    TicketId = newTicket.Id,
                    Property = "",
                    OldValue = "",
                    NewValue = "",
                    Created = DateTimeOffset.UtcNow,
                    UserId = userId,
                    Description = "New Ticket Created"
                };

                try
                {
                    await _context.TicketHistories.AddAsync(history);
                    await _context.SaveChangesAsync();
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"***** ERROR ***** - Error Creating New Ticket History. ---> {ex.Message}");
                    throw;
                }
            }
            else
            {
                // Check Title
                if (oldTicket.Title != newTicket.Title)
                {
                    TicketHistory history = new()
                    {
                        TicketId = newTicket.Id,
                        Property = "Title",
                        OldValue = oldTicket.Title,
                        NewValue = newTicket.Title,
                        Created = DateTimeOffset.UtcNow,
                        UserId = userId,
                        Description = $"New Ticket Title: {newTicket.Title}"
                    };

                    await _context.TicketHistories.AddAsync(history);
                }
                
                // Check Description
                if (oldTicket.Description != newTicket.Description)
                {
                    TicketHistory history = new()
                    {
                        TicketId = newTicket.Id,
                        Property = "Description",
                        OldValue = oldTicket.Description,
                        NewValue = newTicket.Description,
                        Created = DateTimeOffset.UtcNow,
                        UserId = userId,
                        Description = $"New Ticket Description: {newTicket.Description}"
                    };

                    await _context.TicketHistories.AddAsync(history);
                }

                // Check Priority
                if (oldTicket.TicketPriorityId != newTicket.TicketPriorityId)
                {
                    TicketHistory history = new()
                    {
                        TicketId = newTicket.Id,
                        Property = "TicketPriority",
                        OldValue = oldTicket.TicketPriority.Name,
                        NewValue = newTicket.TicketPriority.Name,
                        Created = DateTimeOffset.UtcNow,
                        UserId = userId,
                        Description = $"New Ticket Priority: {newTicket.TicketPriority.Name}"
                    };

                    await _context.TicketHistories.AddAsync(history);
                }

                // Check Status
                if (oldTicket.TicketStatusId != newTicket.TicketStatusId)
                {
                    TicketHistory history = new()
                    {
                        TicketId = newTicket.Id,
                        Property = "TicketStatus",
                        OldValue = oldTicket.TicketStatus.Name,
                        NewValue = newTicket.TicketStatus.Name,
                        Created = DateTimeOffset.UtcNow,
                        UserId = userId,
                        Description = $"New Ticket Status: {newTicket.TicketStatus.Name}"
                    };

                    await _context.TicketHistories.AddAsync(history);
                }

                // Check Type
                if (oldTicket.TicketTypeId != newTicket.TicketTypeId)
                {
                    TicketHistory history = new()
                    {
                        TicketId = newTicket.Id,
                        Property = "TicketType",
                        OldValue = oldTicket.TicketType.Name,
                        NewValue = newTicket.TicketType.Name,
                        Created = DateTimeOffset.UtcNow,
                        UserId = userId,
                        Description = $"New Ticket Type: {newTicket.TicketType.Name}"
                    };

                    await _context.TicketHistories.AddAsync(history);
                }

                // Check Developer
                if (oldTicket.DeveloperUserId != newTicket.DeveloperUserId)
                {
                    TicketHistory history = new()
                    {
                        TicketId = newTicket.Id,
                        Property = "Developer",
                        OldValue = oldTicket.DeveloperUser?.FullName ?? "Not Assigned",
                        NewValue = newTicket.DeveloperUser?.FullName,
                        Created = DateTimeOffset.UtcNow,
                        UserId = userId,
                        Description = $"New Ticket Developer: {newTicket.DeveloperUser.FullName}"
                    };

                    await _context.TicketHistories.AddAsync(history);
                }

                try
                {
                    // Save the TicketHistory DbSet to the database
                    await _context.SaveChangesAsync();
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"***** ERROR ***** - Error Updating Ticket History. ---> {ex.Message}");
                    throw;
                }
            }
        }
        
        public async Task AddHistoryAsync(int ticketId, string model, string userId)
        {
            try
            {
                Ticket ticket = await _context.Tickets.FindAsync(ticketId);
                string description = model.ToLower().Replace("ticket", "");
                description = $"New {description} added to ticket: {ticket.Title}";

                TicketHistory history = new()
                {
                    TicketId = ticket.Id,
                    Property = model,
                    OldValue = "",
                    NewValue = "",
                    Created = DateTimeOffset.UtcNow,
                    UserId = userId,
                    Description = description
                };

                await _context.TicketHistories.AddAsync(history);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"***** ERROR ***** - Error Updating Ticket History. ---> {ex.Message}");
                throw;
            }
        }

        public async Task<List<TicketHistory>> GetCompanyTicketsHistoriesAsync(int companyId)
        {
            try
            {
                List<Project> projects = (await _context.Companies
                    .Include(c => c.Projects)
                        .ThenInclude(p => p.Tickets)
                            .ThenInclude(t => t.History)
                                .ThenInclude(h => h.User)
                    .AsSplitQuery()
                    .FirstOrDefaultAsync(c => c.Id == companyId))!
                    .Projects.ToList();

                List<Ticket> tickets = projects.SelectMany(p => p.Tickets).ToList();

                List<TicketHistory> ticketHistories = tickets.SelectMany(t => t.History).ToList();

                return ticketHistories;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"***** ERROR ***** - Error Getting Company Tickets History. ---> {ex.Message}");
                throw;
            }
        }

        public async Task<List<TicketHistory>> GetProjectTicketsHistoriesAsync(int projectId, int companyId)
        {
            try
            {
                Project project = await _context.Projects.Where(p => p.CompanyId == companyId)
                    .Include(p => p.Tickets)
                        .ThenInclude(t => t.History)
                            .ThenInclude(h => h.User)
                    .AsSplitQuery()
                    .FirstOrDefaultAsync(p => p.Id == projectId);

                List<TicketHistory> ticketHistories = project.Tickets.SelectMany(t => t.History).ToList();

                return ticketHistories;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"***** ERROR ***** - Error Getting Project Tickets History. ---> {ex.Message}");
                throw;
            }
        }
    }
}
