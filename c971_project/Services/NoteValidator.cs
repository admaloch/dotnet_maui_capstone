using c971_project.Helpers;
using c971_project.Messages;
using c971_project.Models;
using c971_project.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Threading.Tasks;

namespace c971_project.Services
{

    public static class NoteValidator
    {
        public static StringBuilder ValidateNote(Note note)
        {
            note.Validate();

            var errorBuilder = new StringBuilder();

            // term errors
            errorBuilder.AppendLine(ValidationHelper.GetErrors(
                note, nameof(note.Title), nameof(note.Body)));

            return errorBuilder;
        }
    }
}