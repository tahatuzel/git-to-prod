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
  type: number
}

export interface ApiEnvelope<T> {
  result: {
    isSuccess: boolean
  }
  response: T | null
  errorResponse: {
    errors: ApiError[]
  } | null
}

export type TodoFilter = 'all' | 'open' | 'completed'
