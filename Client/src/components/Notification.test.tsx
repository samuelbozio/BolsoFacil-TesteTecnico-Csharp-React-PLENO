import { describe, it, expect, beforeEach, vi } from 'vitest'
import { render, screen } from '@testing-library/react'
import Notification from '@/components/Notification'
import LoggerService from '@/services/logger'

describe('Notification Component', () => {
  const mockOnClose = vi.fn()

  beforeEach(() => {
    mockOnClose.mockClear()
    LoggerService.debug('Iniciando teste de Notification', 'NOTIFICATION_TEST')
  })

  it('should render notification with message', () => {
    LoggerService.debug('Testando render de notification com mensagem', 'NOTIFICATION_TEST')
    render(
      <Notification 
        id="1"
        message="Test notification" 
        type="success"
        onClose={mockOnClose}
      />
    )
    const notification = screen.getByText('Test notification')
    expect(notification).toBeInTheDocument()
    LoggerService.debug('Notification renderizada com sucesso', 'NOTIFICATION_TEST')
  })

  it('should render success notification type', () => {
    LoggerService.debug('Testando tipo success', 'NOTIFICATION_TEST')
    const { container } = render(
      <Notification 
        id="1"
        message="Success" 
        type="success"
        onClose={mockOnClose}
      />
    )
    expect(container.querySelector('.alert-success')).toBeTruthy()
    LoggerService.debug('Tipo success renderizado corretamente', 'NOTIFICATION_TEST')
  })

  it('should render error notification type', () => {
    LoggerService.debug('Testando tipo error', 'NOTIFICATION_TEST')
    const { container } = render(
      <Notification 
        id="1"
        message="Error" 
        type="error"
        onClose={mockOnClose}
      />
    )
    expect(container.querySelector('.alert-error')).toBeTruthy()
    LoggerService.debug('Tipo error renderizado corretamente', 'NOTIFICATION_TEST')
  })

  it('should render warning notification type', () => {
    LoggerService.debug('Testando tipo warning', 'NOTIFICATION_TEST')
    const { container } = render(
      <Notification 
        id="1"
        message="Warning" 
        type="warning"
        onClose={mockOnClose}
      />
    )
    expect(container.querySelector('.alert-warning')).toBeTruthy()
    LoggerService.debug('Tipo warning renderizado corretamente', 'NOTIFICATION_TEST')
  })

  it('should handle empty message', () => {
    LoggerService.debug('Testando mensagem vazia', 'NOTIFICATION_TEST')
    const { container } = render(
      <Notification 
        id="1"
        message="" 
        type="info"
        onClose={mockOnClose}
      />
    )
    const notification = container.querySelector('.alert')
    expect(notification).toBeTruthy()
    LoggerService.debug('Mensagem vazia tratada corretamente', 'NOTIFICATION_TEST')
  })
})
