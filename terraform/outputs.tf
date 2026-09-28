output "load_balancer_dns_name" {
  description = "Public DNS name for the application load balancer."
  value       = aws_lb.app.dns_name
}

output "application_url" {
  description = "Public HTTP link after the workers serve the application on port 80."
  value       = "http://${aws_lb.app.dns_name}"
}

output "instance_ids" {
  description = "Use these IDs as SSH-over-SSM targets."
  value       = { for name, instance in aws_instance.nodes : name => instance.id }
}

output "private_ips" {
  description = "Private addresses for K3s and PostgreSQL configuration."
  value       = { for name, instance in aws_instance.nodes : name => instance.private_ip }
}
