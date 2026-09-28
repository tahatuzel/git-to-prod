export interface TodoItem {
  id: number
  title: string
  description: string
  isCompleted: boolean
}

export interface TodoItemInput {
  title: string
  description: string
  isCompleted: boolean
}

export interface ApiError {
  code: string
  message: string
  propertyName: string | null
}

export interface ApiErrorResponse {
  errors: ApiError[]
}

export type TodoFilter = 'all' | 'open' | 'completed'
