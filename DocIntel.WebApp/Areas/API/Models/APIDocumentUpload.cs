/* DocIntel
 * Copyright (C) 2018-2021 Belgian Defense, Antoine Cailliau
 *
 * This program is free software: you can redistribute it and/or modify
 * it under the terms of the GNU Affero General Public License as published by
 * the Free Software Foundation, either version 3 of the License, or
 * (at your option) any later version.
 *
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU Affero General Public License for more details.
 *
 * You should have received a copy of the GNU Affero General Public License
 * along with this program.  If not, see <http://www.gnu.org/licenses/>.
*/

using System;

namespace DocIntel.WebApp.Areas.API.Models
{
    public class APIDocumentUpload
    {
        public string ExternalReference { get; set; }

        public string Title { get; set; }

        public string ShortDescription { get; set; }

        public string SourceName { get; set; }

        public Guid SourceId { get; set; }

        public string SourceUrl { get; set; }

        public Guid ClassificationId { get; set; }

        public string Note { get; set; }

        public bool SkipObservables { get; set; }

        public DateTime RegistrationDate { get; set; }

        public DateTime ModificationDate { get; set; }

        public DateTime DocumentDate { get; set; }

        public string[] Tags { get; set; }

        public int Status { get; set; }
    }
}