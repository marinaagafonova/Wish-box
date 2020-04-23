using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Wish_Box.Migrations
{
    public partial class UpdateCommment : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            //migrationBuilder.CreateTable(
            //    name: "Followings",
            //    columns: table => new
            //    {
            //        Id = table.Column<int>(nullable: false)
            //            .Annotation("SqlServer:Identity", "1, 1"),
            //        UserFId = table.Column<int>(nullable: false),
            //        UserIsFId = table.Column<int>(nullable: false)
            //    },
            //    constraints: table =>
            //    {
            //        table.PrimaryKey("PK_Followings", x => x.Id);
            //    });

            //migrationBuilder.CreateTable(
            //    name: "Users",
            //    columns: table => new
            //    {
            //        Id = table.Column<int>(nullable: false)
            //            .Annotation("SqlServer:Identity", "1, 1"),
            //        Login = table.Column<string>(nullable: true),
            //        Password = table.Column<string>(nullable: true),
            //        dayOfBirth = table.Column<DateTime>(nullable: false),
            //        Country = table.Column<string>(nullable: true),
            //        City = table.Column<string>(nullable: true),
            //        Avatar = table.Column<byte[]>(nullable: true)
            //    },
            //    constraints: table =>
            //    {
            //        table.PrimaryKey("PK_Users", x => x.Id);
            //    });

            //migrationBuilder.CreateTable(
            //    name: "WishRatings",
            //    columns: table => new
            //    {
            //        Id = table.Column<int>(nullable: false)
            //            .Annotation("SqlServer:Identity", "1, 1"),
            //        WishId = table.Column<int>(nullable: false),
            //        UserId = table.Column<int>(nullable: false),
            //        Rate = table.Column<bool>(nullable: false)
            //    },
            //    constraints: table =>
            //    {
            //        table.PrimaryKey("PK_WishRatings", x => x.Id);
            //    });

            //migrationBuilder.CreateTable(
            //    name: "Wishes",
            //    columns: table => new
            //    {
            //        Id = table.Column<int>(nullable: false)
            //            .Annotation("SqlServer:Identity", "1, 1"),
            //        Description = table.Column<string>(nullable: true),
            //        IsTaken = table.Column<bool>(nullable: false),
            //        UserId = table.Column<int>(nullable: false),
            //        Attachment = table.Column<byte[]>(nullable: true),
            //        Rating = table.Column<int>(nullable: false)
            //    },
            //    constraints: table =>
            //    {
            //        table.PrimaryKey("PK_Wishes", x => x.Id);
            //        table.ForeignKey(
            //            name: "FK_Wishes_Users_UserId",
            //            column: x => x.UserId,
            //            principalTable: "Users",
            //            principalColumn: "Id",
            //            onDelete: ReferentialAction.Cascade);
            //    });

            //migrationBuilder.CreateTable(
            //    name: "Comments",
            //    columns: table => new
            //    {
            //        Id = table.Column<int>(nullable: false)
            //            .Annotation("SqlServer:Identity", "1, 1"),
            //        Description = table.Column<string>(nullable: true),
            //        WishId = table.Column<int>(nullable: false),
            //        InReplyId = table.Column<int>(nullable: true),
            //        UserId = table.Column<int>(nullable: false)
            //    },
            //    constraints: table =>
            //    {
            //        table.PrimaryKey("PK_Comments", x => x.Id);
            //        table.ForeignKey(
            //            name: "FK_Comments_Comments_InReplyId",
            //            column: x => x.InReplyId,
            //            principalTable: "Comments",
            //            principalColumn: "Id",
            //            onDelete: ReferentialAction.Restrict);
            //        table.ForeignKey(
            //            name: "FK_Comments_Users_UserId",
            //            column: x => x.UserId,
            //            principalTable: "Users",
            //            principalColumn: "Id",
            //            onDelete: ReferentialAction.Restrict);
            //        table.ForeignKey(
            //            name: "FK_Comments_Wishes_WishId",
            //            column: x => x.WishId,
            //            principalTable: "Wishes",
            //            principalColumn: "Id",
            //            onDelete: ReferentialAction.Restrict);
            //    });

            //migrationBuilder.CreateTable(
            //    name: "TakenWishes",
            //    columns: table => new
            //    {
            //        Id = table.Column<int>(nullable: false)
            //            .Annotation("SqlServer:Identity", "1, 1"),
            //        IsGiven = table.Column<bool>(nullable: false),
            //        WishId = table.Column<int>(nullable: false),
            //        WhoWishesId = table.Column<int>(nullable: false),
            //        WhoGivesId = table.Column<int>(nullable: false)
            //    },
            //    constraints: table =>
            //    {
            //        table.PrimaryKey("PK_TakenWishes", x => x.Id);
            //        table.ForeignKey(
            //            name: "FK_TakenWishes_Wishes_WishId",
            //            column: x => x.WishId,
            //            principalTable: "Wishes",
            //            principalColumn: "Id",
            //            onDelete: ReferentialAction.Cascade);
            //    });

            //migrationBuilder.CreateIndex(
            //    name: "IX_Comments_InReplyId",
            //    table: "Comments",
            //    column: "InReplyId");

            //migrationBuilder.CreateIndex(
            //    name: "IX_Comments_UserId",
            //    table: "Comments",
            //    column: "UserId");

            //migrationBuilder.CreateIndex(
            //    name: "IX_Comments_WishId",
            //    table: "Comments",
            //    column: "WishId");

            //migrationBuilder.CreateIndex(
            //    name: "IX_TakenWishes_WishId",
            //    table: "TakenWishes",
            //    column: "WishId");

            //migrationBuilder.CreateIndex(
            //    name: "IX_Wishes_UserId",
            //    table: "Wishes",
            //    column: "UserId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            //migrationBuilder.DropTable(
            //    name: "Comments");

            //migrationBuilder.DropTable(
            //    name: "Followings");

            //migrationBuilder.DropTable(
            //    name: "TakenWishes");

            //migrationBuilder.DropTable(
            //    name: "WishRatings");

            //migrationBuilder.DropTable(
            //    name: "Wishes");

            //migrationBuilder.DropTable(
            //    name: "Users");
        }
    }
}
