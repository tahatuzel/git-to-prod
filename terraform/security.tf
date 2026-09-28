resource "aws_security_group" "private_nodes" {
  name_prefix = "${var.project_name}-nodes-"
  description = "Shared private traffic between the four VMs"
  vpc_id      = aws_vpc.main.id

  tags = { Name = "${var.project_name}-nodes" }
}

resource "aws_vpc_security_group_ingress_rule" "node_to_node" {
  security_group_id            = aws_security_group.private_nodes.id
  referenced_security_group_id = aws_security_group.private_nodes.id
  ip_protocol                  = "-1"
  description                  = "Temporary full access among the four private VMs"
}

resource "aws_vpc_security_group_egress_rule" "node_outbound" {
  security_group_id = aws_security_group.private_nodes.id
  cidr_ipv4         = "0.0.0.0/0"
  ip_protocol       = "-1"
  description       = "Outbound internet access through NAT"
}

resource "aws_security_group" "workers" {
  name_prefix = "${var.project_name}-workers-"
  description = "Application entry on K3s workers"
  vpc_id      = aws_vpc.main.id

  tags = { Name = "${var.project_name}-workers" }
}

resource "aws_vpc_security_group_ingress_rule" "alb_to_workers" {
  security_group_id            = aws_security_group.workers.id
  referenced_security_group_id = aws_security_group.alb.id
  ip_protocol                  = "tcp"
  from_port                    = 80
  to_port                      = 80
  description                  = "HTTP from the application load balancer"
}

resource "aws_security_group" "alb" {
  name_prefix = "${var.project_name}-alb-"
  description = "Public application load balancer"
  vpc_id      = aws_vpc.main.id

  tags = { Name = "${var.project_name}-alb" }
}

resource "aws_vpc_security_group_ingress_rule" "alb_http" {
  security_group_id = aws_security_group.alb.id
  cidr_ipv4         = "0.0.0.0/0"
  ip_protocol       = "tcp"
  from_port         = 80
  to_port           = 80
  description       = "Public HTTP"
}

resource "aws_vpc_security_group_egress_rule" "alb_to_workers" {
  security_group_id            = aws_security_group.alb.id
  referenced_security_group_id = aws_security_group.workers.id
  ip_protocol                  = "tcp"
  from_port                    = 80
  to_port                      = 80
  description                  = "Application traffic to K3s workers"
}
