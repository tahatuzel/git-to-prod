resource "aws_lb" "app" {
  name               = "${var.project_name}-alb"
  internal           = false
  load_balancer_type = "application"
  security_groups    = [aws_security_group.alb.id]
  subnets            = aws_subnet.public[*].id
}

resource "aws_lb_target_group" "workers" {
  name        = "${var.project_name}-workers"
  port        = 80
  protocol    = "HTTP"
  target_type = "instance"
  vpc_id      = aws_vpc.main.id

  health_check {
    path    = "/"
    matcher = "200-399"
  }
}

resource "aws_lb_target_group_attachment" "worker_1" {
  target_group_arn = aws_lb_target_group.workers.arn
  target_id        = aws_instance.nodes["k3s-worker-1"].id
  port             = 80
}

resource "aws_lb_target_group_attachment" "worker_2" {
  target_group_arn = aws_lb_target_group.workers.arn
  target_id        = aws_instance.nodes["k3s-worker-2"].id
  port             = 80
}

resource "aws_lb_listener" "http" {
  load_balancer_arn = aws_lb.app.arn
  port              = 80
  protocol          = "HTTP"

  default_action {
    type             = "forward"
    target_group_arn = aws_lb_target_group.workers.arn
  }
}
