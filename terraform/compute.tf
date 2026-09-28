data "aws_ssm_parameter" "amazon_linux_2023" {
  name = "/aws/service/ami-amazon-linux-latest/al2023-ami-kernel-default-x86_64"
}

resource "aws_iam_role" "ssm" {
  name_prefix = "${var.project_name}-ssm-"

  assume_role_policy = jsonencode({
    Version = "2012-10-17"
    Statement = [{
      Action = "sts:AssumeRole"
      Effect = "Allow"
      Principal = {
        Service = "ec2.amazonaws.com"
      }
    }]
  })
}

resource "aws_iam_role_policy_attachment" "ssm" {
  role       = aws_iam_role.ssm.name
  policy_arn = "arn:aws:iam::aws:policy/AmazonSSMManagedInstanceCore"
}

resource "aws_iam_instance_profile" "ssm" {
  name_prefix = "${var.project_name}-ssm-"
  role        = aws_iam_role.ssm.name
}

locals {
  nodes = {
    k3s-server   = { subnet = 0, worker = false, root_gb = 30 }
    k3s-worker-1 = { subnet = 0, worker = true, root_gb = 30 }
    k3s-worker-2 = { subnet = 1, worker = true, root_gb = 30 }
    db           = { subnet = 1, worker = false, root_gb = 50 }
  }
}

resource "aws_instance" "nodes" {
  for_each = local.nodes

  ami                         = data.aws_ssm_parameter.amazon_linux_2023.value
  instance_type               = each.key == "db" ? var.db_instance_type : var.k3s_instance_type
  subnet_id                   = aws_subnet.private[each.value.subnet].id
  vpc_security_group_ids      = each.value.worker ? [aws_security_group.private_nodes.id, aws_security_group.workers.id] : [aws_security_group.private_nodes.id]
  associate_public_ip_address = false
  key_name                    = var.ssh_key_name
  iam_instance_profile        = aws_iam_instance_profile.ssm.name

  root_block_device {
    volume_type = "gp3"
    volume_size = each.value.root_gb
    encrypted   = true
  }

  metadata_options {
    http_tokens = "required"
  }

  depends_on = [aws_iam_role_policy_attachment.ssm]
  tags = { Name = "${var.project_name}-${each.key}" }
}
