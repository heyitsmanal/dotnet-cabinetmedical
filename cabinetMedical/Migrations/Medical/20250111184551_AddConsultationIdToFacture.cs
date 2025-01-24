using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace cabinetMedical.Migrations.Medical
{
    public partial class AddConsultationIdToFacture : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Drop foreign key constraints before modifying tables
            migrationBuilder.DropForeignKey(
                name: "FK__Consultat__Dossi__5165187F",
                table: "Consultation");

            migrationBuilder.DropForeignKey(
                name: "FK__Consultat__Médec__5070F446",
                table: "Consultation");

            migrationBuilder.DropForeignKey(
                name: "FK__Consultat__Patie__4F7CD00D",
                table: "Consultation");

            migrationBuilder.DropForeignKey(
                name: "FK__DossierMé__Médec__4CA06362",
                table: "DossierMedical");

            migrationBuilder.DropForeignKey(
                name: "FK__DossierMé__Patie__4BAC3F29",
                table: "DossierMedical");

            migrationBuilder.DropForeignKey(
                name: "FK__Médicamen__Consu__5441852A",
                table: "Medicament");

            // Add the ConsultationId column to Factures
            migrationBuilder.AddColumn<int>(
                name: "ConsultationId",
                table: "Factures",
                type: "int",
                nullable: false,
                defaultValue: 0);

            // Add foreign key constraint for ConsultationId in Factures
            migrationBuilder.AddForeignKey(
                name: "FK_Factures_Consultation_ConsultationId",
                table: "Factures",
                column: "ConsultationId",
                principalTable: "Consultation",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            // Re-add all other foreign key constraints
            migrationBuilder.AddForeignKey(
                name: "FK_Consultation_DossierMedical_DossierMédicalId",
                table: "Consultation",
                column: "DossierMédicalId",
                principalTable: "DossierMedical",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Consultation_Medecin_MédecinId",
                table: "Consultation",
                column: "MédecinId",
                principalTable: "Medecin",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Consultation_Patient_PatientId",
                table: "Consultation",
                column: "PatientId",
                principalTable: "Patient",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_DossierMedical_Medecin_MédecinId",
                table: "DossierMedical",
                column: "MédecinId",
                principalTable: "Medecin",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_DossierMedical_Patient_PatientId",
                table: "DossierMedical",
                column: "PatientId",
                principalTable: "Patient",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Medicament_Consultation_ConsultationId",
                table: "Medicament",
                column: "ConsultationId",
                principalTable: "Consultation",
                principalColumn: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // Drop the foreign key constraint for ConsultationId
            migrationBuilder.DropForeignKey(
                name: "FK_Factures_Consultation_ConsultationId",
                table: "Factures");

            // Remove the ConsultationId column from Factures
            migrationBuilder.DropColumn(
                name: "ConsultationId",
                table: "Factures");

            // Re-add all other foreign key constraints
            migrationBuilder.AddForeignKey(
                name: "FK_Consultation_DossierMedical_DossierMédicalId",
                table: "Consultation",
                column: "DossierMédicalId",
                principalTable: "DossierMedical",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Consultation_Medecin_MédecinId",
                table: "Consultation",
                column: "MédecinId",
                principalTable: "Medecin",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Consultation_Patient_PatientId",
                table: "Consultation",
                column: "PatientId",
                principalTable: "Patient",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_DossierMedical_Medecin_MédecinId",
                table: "DossierMedical",
                column: "MédecinId",
                principalTable: "Medecin",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_DossierMedical_Patient_PatientId",
                table: "DossierMedical",
                column: "PatientId",
                principalTable: "Patient",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Medicament_Consultation_ConsultationId",
                table: "Medicament",
                column: "ConsultationId",
                principalTable: "Consultation",
                principalColumn: "Id");
        }
    }
}
