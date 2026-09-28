variable "aws_region" {
  description = "AWS region for all resources."
  type        = string
}

variable "project_name" {
  description = "Prefix for resource names and Project tags."
  type        = string
  default     = "git-to-prod"

  validation {
    condition     = can(regex("^[a-z0-9-]{1,20}$", var.project_name))
    error_message = "project_name must contain 1-20 lowercase letters, numbers or hyphens."
  }
}

variable "ssh_key_name" {
  description = "Name of an existing EC2 key pair in aws_region; its private key stays outside Terraform."
  type        = string

  validation {
    condition     = length(trimspace(var.ssh_key_name)) > 0
    error_message = "ssh_key_name must not be empty."
  }
}

variable "vpc_cidr" {
  description = "IPv4 CIDR for the VPC; the default creates /24 public and private subnets."
  type        = string
  default     = "10.42.0.0/16"
}

variable "k3s_instance_type" {
  description = "EC2 instance type for the K3s server and workers."
  type        = string
  default     = "t3.medium"
}

variable "db_instance_type" {
  description = "EC2 instance type for the database VM."
  type        = string
  default     = "t3.medium"
}
