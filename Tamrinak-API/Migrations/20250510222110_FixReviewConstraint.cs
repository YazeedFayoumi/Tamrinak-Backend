using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Tamrinak_API.Migrations
{
    /// <inheritdoc />
    public partial class FixReviewConstraint : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("ALTER TABLE Reviews DROP CONSTRAINT CK_Review_SingleTarget");

            migrationBuilder.Sql(@"
        ALTER TABLE Reviews
        ADD CONSTRAINT CK_Review_SingleTarget
        CHECK (
            (FacilityId IS NOT NULL AND FieldId IS NULL AND ParentReviewId IS NULL) OR
            (FacilityId IS NULL AND FieldId IS NOT NULL AND ParentReviewId IS NULL) OR
            (FacilityId IS NULL AND FieldId IS NULL AND ParentReviewId IS NOT NULL)
        )
    ");

        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("ALTER TABLE Reviews DROP CONSTRAINT CK_Review_SingleTarget");

            migrationBuilder.Sql(@"
        ALTER TABLE Reviews
        ADD CONSTRAINT CK_Review_SingleTarget
        CHECK (
            (FacilityId IS NOT NULL AND FieldId IS NULL AND ParentReviewId IS NULL) OR
            (FacilityId IS NULL AND FieldId IS NOT NULL AND ParentReviewId IS NULL)
        )
    ");

        }
    }
}
