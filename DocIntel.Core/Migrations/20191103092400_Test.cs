/*
 * DocIntel
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

using Microsoft.EntityFrameworkCore.Migrations;

namespace DocIntel.Core.Migrations
{
    public partial class Test : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                "FK_Documents_Sources_SourceId",
                "Documents");

            migrationBuilder.AlterColumn<int>(
                "SourceId",
                "Documents",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.AddForeignKey(
                "FK_Documents_Sources_SourceId",
                "Documents",
                "SourceId",
                "Sources",
                principalColumn: "SourceId",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                "FK_Documents_Sources_SourceId",
                "Documents");

            migrationBuilder.AlterColumn<int>(
                "SourceId",
                "Documents",
                "integer",
                nullable: false,
                oldClrType: typeof(int),
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                "FK_Documents_Sources_SourceId",
                "Documents",
                "SourceId",
                "Sources",
                principalColumn: "SourceId",
                onDelete: ReferentialAction.Cascade);
        }
    }
}