using Domain.Models;
using Microsoft.EntityFrameworkCore;

namespace Infra.DB;

public class MyDBContext : DbContext
{
	public DbSet<Notifications> Notifications { get; set; }
	public DbSet<Subscriptions> Subscriptions { get; set; }

	public MyDBContext(DbContextOptions<MyDBContext> options) : base(options) { }

	protected override void OnModelCreating(ModelBuilder modelBuilder)
	{
		modelBuilder.Entity<Notifications>()
			.HasOne<Subscriptions>()
			.WithMany()
			.HasForeignKey(n => n.SubscriptionId)
			.IsRequired();

		modelBuilder.Entity<Subscriptions>(entity =>
		{
			entity.HasOne<MainTask>()
				.WithMany()
				.HasForeignKey(s => s.MainTaskIdTopic)
				.IsRequired();

			entity.HasOne<SubTask>()
				.WithMany()
				.HasForeignKey(s => s.SubTaskIdSubscriber)
				.IsRequired();
		});

		base.OnModelCreating(modelBuilder);
	}

}