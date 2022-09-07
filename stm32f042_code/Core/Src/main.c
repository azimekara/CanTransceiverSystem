/* USER CODE BEGIN Header */
/**
  **************************
  * @file           : main.c
  * @brief          : Main program body
  **************************
  * @attention
  *
  * Copyright (c) 2022 STMicroelectronics.
  * All rights reserved.
  *
  * This software is licensed under terms that can be found in the LICENSE file
  * in the root directory of this software component.
  * If no LICENSE file comes with this software, it is provided AS-IS.
  *
  **************************
  */
/* USER CODE END Header */
/* Includes ------------------------------------------------------------------*/
#include "main.h"

/* Private includes ----------------------------------------------------------*/
/* USER CODE BEGIN Includes */
#include "string.h"

/* USER CODE END Includes */

/* Private typedef -----------------------------------------------------------*/
/* USER CODE BEGIN PTD */

/* USER CODE END PTD */

/* Private define ------------------------------------------------------------*/
/* USER CODE BEGIN PD */
/* USER CODE END PD */

/* Private macro -------------------------------------------------------------*/
/* USER CODE BEGIN PM */

/* USER CODE END PM */

/* Private variables ---------------------------------------------------------*/
 CAN_HandleTypeDef hcan;

TIM_HandleTypeDef htim17;

UART_HandleTypeDef huart1;

/* USER CODE BEGIN PV */
uint8_t 	 RxData[21];
uint8_t 	 TxData[21];
unsigned int baudrateThousand,
			 baudrateHundred,
			 baudrateTen,
			 baudrateUnit;
uint8_t      myIDstring[3];
unsigned int myID,DLCval;
unsigned int canBaudrate;
uint32_t     i =0;
uint8_t  	 Data1String[2],
		 	 Data2String[2],
			 Data3String[2],
			 Data4String[2],
			 Data5String[2],
			 Data6String[2],
			 Data7String[2],
			 Data8String[2];
unsigned int Data1,
			 Data2,
			 Data3,
			 Data4,
			 Data5,
			 Data6,
			 Data7,
			 Data8;
CAN_TxHeaderTypeDef TxHeader;
CAN_RxHeaderTypeDef RxHeader;
uint32_t TxMailbox;
uint8_t canTxData[8],canRxData[8];
/* USER CODE END PV */

/* Private function prototypes -----------------------------------------------*/
void SystemClock_Config(void);
static void MX_GPIO_Init(void);
static void MX_TIM17_Init(void);
static void MX_CAN_Init(void);
static void MX_USART1_UART_Init(void);
/* USER CODE BEGIN PFP */
void HAL_TIM_PeriodElapsedCallback(TIM_HandleTypeDef *htim)
{
  i++;
}
void HAL_UART_RxCpltCallback(UART_HandleTypeDef *huart)
{
	 HAL_UART_Receive_IT(&huart1, RxData, 21);
	 if(RxData[4] == '*')
	 {
		 sscanf(&RxData[0],"%01d",&baudrateThousand);
		 sscanf(&RxData[1],"%01d",&baudrateHundred);
		 sscanf(&RxData[2],"%01d",&baudrateTen);
		 sscanf(&RxData[3],"%01d",&baudrateUnit);

		 canBaudrate = (1000*baudrateThousand) + (100*baudrateHundred) + (10*baudrateTen) + (baudrateUnit);
		 hcan.Init.Prescaler = 48000.0 / (canBaudrate * 16);
		 memset(RxData,0,21);
	 }

	 sprintf(myIDstring,"%c%c%c",RxData[0],RxData[1],RxData[2]);
	 sscanf(myIDstring,"%03X",&myID);
	 TxHeader.IDE=CAN_ID_STD;
	 TxHeader.StdId = myID;

	 if(RxData[3] == 0)
	 {
		 TxHeader.RTR = CAN_RTR_DATA;
		 TxHeader.DLC = RxData[4];
		 sprintf(Data1String,"%c%c",RxData[5],RxData[6]);
		 sscanf(Data1String,"%02X",&Data1);
		 sprintf(Data2String,"%c%c",RxData[7],RxData[8]);
		 sscanf(Data2String,"%02X",&Data2);
		 sprintf(Data3String,"%c%c",RxData[9],RxData[10]);
		 sscanf(Data3String,"%02X",&Data3);
		 sprintf(Data4String,"%c%c",RxData[11],RxData[12]);
		 sscanf(Data4String,"%02X",&Data4);
		 sprintf(Data5String,"%c%c",RxData[13],RxData[14]);
		 sscanf(Data5String,"%02X",&Data5);
		 sprintf(Data6String,"%c%c",RxData[15],RxData[16]);
		 sscanf(Data6String,"%02X",&Data6);
		 sprintf(Data7String,"%c%c",RxData[17],RxData[18]);
		 sscanf(Data7String,"%02X",&Data7);
		 sprintf(Data7String,"%c%c",RxData[19],RxData[20]);
		 sscanf(Data8String,"%02X",&Data8);
		 canTxData[0] = Data1;
		 canTxData[1] = Data2;
		 canTxData[2] = Data3;
		 canTxData[3] = Data4;
		 canTxData[4] = Data5;
		 canTxData[5] = Data6;
		 canTxData[6] = Data7;
		 canTxData[7] = Data8;
		if(HAL_CAN_AddTxMessage(&hcan, &TxHeader, canTxData, &TxMailbox) != HAL_OK)
		{
			HAL_TIM_Base_Start_IT(&htim17);
				if (i >= 40)
				{
					Error_Handler();
				}
		}
		memset(RxData,0,21);
	 }else if(RxData[3] == 1)
	 {
		 TxHeader.RTR = CAN_RTR_REMOTE;
		 TxHeader.DLC = 0;
		 if(HAL_CAN_AddTxMessage(&hcan, &TxHeader, canTxData, &TxMailbox) != HAL_OK)
		 {
		 	    HAL_TIM_Base_Start_IT(&htim17);
		 		if (i >= 40)
		 		{
		 			Error_Handler();
		        }
		 }
		 memset(RxData,0,21);
	 }

}

void HAL_CAN_RxFifo0MsgPendingCallback(CAN_HandleTypeDef *hcan)
{
 HAL_CAN_GetRxMessage(hcan, CAN_RX_FIFO0, &RxHeader, canRxData);

 if(RxHeader.RTR == CAN_RTR_REMOTE)
 {
	 sprintf(TxData,"%03X100000000000000000",RxHeader.StdId);
	 HAL_UART_Transmit_IT(&huart1, TxData, sprintf(TxData,"%03X100000000000000000",RxHeader.StdId));
 }else if(RxHeader.RTR == CAN_RTR_DATA)
 {
	switch(RxHeader.DLC)
	{
	case 0:
		sprintf(TxData,"%03X000000000000000000",RxHeader.StdId);
		HAL_UART_Transmit_IT(&huart1, TxData, sprintf(TxData,"%03X000000000000000000",RxHeader.StdId));
		break;
	case 1:
		sprintf(TxData,"%03X01%02X00000000000000",RxHeader.StdId,canRxData[0]);
		HAL_UART_Transmit_IT(&huart1, TxData, sprintf(TxData,"%03X01%02X00000000000000",RxHeader.StdId,canRxData[0]));
		break;
	case 2:
		sprintf(TxData,"%03X02%02X%02X000000000000",RxHeader.StdId,canRxData[0],canRxData[1]);
		HAL_UART_Transmit_IT(&huart1, TxData, sprintf(TxData,"%03X02%02X%02X000000000000",RxHeader.StdId,canRxData[0],canRxData[1]));
		break;
	case 3:
		sprintf(TxData,"%03X03%02X%02X%02X0000000000",RxHeader.StdId,canRxData[0],canRxData[1],canRxData[2]);
		HAL_UART_Transmit_IT(&huart1, TxData, sprintf(TxData,"%03X03%02X%02X%02X0000000000",RxHeader.StdId,canRxData[0],canRxData[1],canRxData[2]));
		break;
	case 4:
		sprintf(TxData,"%03X04%02X%02X%02X%02X00000000",RxHeader.StdId,canRxData[0],canRxData[1],canRxData[2],canRxData[3]);
		HAL_UART_Transmit_IT(&huart1, TxData, sprintf(TxData,"%03X04%02X%02X%02X%02X00000000",RxHeader.StdId,canRxData[0],canRxData[1],canRxData[2],canRxData[3]));
		break;
	case 5:
		sprintf(TxData,"%03X05%02X%02X%02X%02X%02X000000",RxHeader.StdId,canRxData[0],canRxData[1],canRxData[2],canRxData[3],canRxData[4]);
		HAL_UART_Transmit_IT(&huart1, TxData, sprintf(TxData,"%03X05%02X%02X%02X%02X%02X000000",RxHeader.StdId,canRxData[0],canRxData[1],canRxData[2],canRxData[3],canRxData[4]));
		break;
	case 6:
		sprintf(TxData,"%03X06%02X%02X%02X%02X%02X%02X0000",RxHeader.StdId,canRxData[0],canRxData[1],canRxData[2],canRxData[3],canRxData[4],canRxData[5]);
		HAL_UART_Transmit_IT(&huart1, TxData, sprintf(TxData,"%03X05%02X%02X%02X%02X%02X%02X0000",RxHeader.StdId,canRxData[0],canRxData[1],canRxData[2],canRxData[3],canRxData[4],canRxData[5]));
		break;
	case 7:
		sprintf(TxData,"%03X07%02X%02X%02X%02X%02X%02X%02X00",RxHeader.StdId,canRxData[0],canRxData[1],canRxData[2],canRxData[3],canRxData[4],canRxData[5],canRxData[6]);
		HAL_UART_Transmit_IT(&huart1, TxData, sprintf(TxData,"%03X07%02X%02X%02X%02X%02X%02X%02X00",RxHeader.StdId,canRxData[0],canRxData[1],canRxData[2],canRxData[3],canRxData[4],canRxData[5],canRxData[6]));
		break;
	case 8:
		sprintf(TxData,"%03X08%02X%02X%02X%02X%02X%02X%02X%02X",RxHeader.StdId,canRxData[0],canRxData[1],canRxData[2],canRxData[3],canRxData[4],canRxData[5],canRxData[6],canRxData[7]);
		HAL_UART_Transmit_IT(&huart1, TxData, sprintf(TxData,"%03X08%02X%02X%02X%02X%02X%02X%02X%02X",RxHeader.StdId,canRxData[0],canRxData[1],canRxData[2],canRxData[3],canRxData[4],canRxData[5],canRxData[6],canRxData[7]));
		break;
	default:
		break;
	}
 }
}
void HAL_UART_TxCpltCallback(UART_HandleTypeDef *huart)
{
	 memset(TxData,0,21);
}
/* USER CODE END PFP */

/* Private user code ---------------------------------------------------------*/
/* USER CODE BEGIN 0 */

/* USER CODE END 0 */

/**
  * @brief  The application entry point.
  * @retval int
  */
int main(void)
{
  /* USER CODE BEGIN 1 */

  /* USER CODE END 1 */

  /* MCU Configuration--------------------------------------------------------*/

  /* Reset of all peripherals, Initializes the Flash interface and the Systick. */
  HAL_Init();

  /* USER CODE BEGIN Init */

  /* USER CODE END Init */

  /* Configure the system clock */
  SystemClock_Config();

  /* USER CODE BEGIN SysInit */

  /* USER CODE END SysInit */

  /* Initialize all configured peripherals */
  MX_GPIO_Init();
  MX_TIM17_Init();
  MX_CAN_Init();
  MX_USART1_UART_Init();
  /* USER CODE BEGIN 2 */

HAL_CAN_Start(&hcan);
HAL_UART_Receive_IT(&huart1, RxData, 21);

HAL_CAN_ActivateNotification(&hcan, CAN_IT_RX_FIFO0_MSG_PENDING);

  /* USER CODE END 2 */

  /* Infinite loop */
  /* USER CODE BEGIN WHILE */
  while (1)
  {
    /* USER CODE END WHILE */

    /* USER CODE BEGIN 3 */
	  HAL_GPIO_WritePin(GPIOB, GPIO_PIN_1|GPIO_PIN_10|GPIO_PIN_11, GPIO_PIN_SET);
  }
  /* USER CODE END 3 */
}

/**
  * @brief System Clock Configuration
  * @retval None
  */
void SystemClock_Config(void)
{
  RCC_OscInitTypeDef RCC_OscInitStruct = {0};
  RCC_ClkInitTypeDef RCC_ClkInitStruct = {0};
  RCC_PeriphCLKInitTypeDef PeriphClkInit = {0};

  /** Initializes the RCC Oscillators according to the specified parameters
  * in the RCC_OscInitTypeDef structure.
  */
  RCC_OscInitStruct.OscillatorType = RCC_OSCILLATORTYPE_HSI48;
  RCC_OscInitStruct.HSI48State = RCC_HSI48_ON;
  RCC_OscInitStruct.PLL.PLLState = RCC_PLL_NONE;
  if (HAL_RCC_OscConfig(&RCC_OscInitStruct) != HAL_OK)
  {
    Error_Handler();
  }

  /** Initializes the CPU, AHB and APB buses clocks
  */
  RCC_ClkInitStruct.ClockType = RCC_CLOCKTYPE_HCLK|RCC_CLOCKTYPE_SYSCLK
                              |RCC_CLOCKTYPE_PCLK1;
  RCC_ClkInitStruct.SYSCLKSource = RCC_SYSCLKSOURCE_HSI48;
  RCC_ClkInitStruct.AHBCLKDivider = RCC_SYSCLK_DIV1;
  RCC_ClkInitStruct.APB1CLKDivider = RCC_HCLK_DIV1;

  if (HAL_RCC_ClockConfig(&RCC_ClkInitStruct, FLASH_LATENCY_1) != HAL_OK)
  {
    Error_Handler();
  }
  PeriphClkInit.PeriphClockSelection = RCC_PERIPHCLK_USART1;
  PeriphClkInit.Usart1ClockSelection = RCC_USART1CLKSOURCE_PCLK1;
  if (HAL_RCCEx_PeriphCLKConfig(&PeriphClkInit) != HAL_OK)
  {
    Error_Handler();
  }
}

/**
  * @brief CAN Initialization Function
  * @param None
  * @retval None
  */
static void MX_CAN_Init(void)
{

  /* USER CODE BEGIN CAN_Init 0 */

  /* USER CODE END CAN_Init 0 */

  /* USER CODE BEGIN CAN_Init 1 */

  /* USER CODE END CAN_Init 1 */
  hcan.Instance = CAN;
  hcan.Init.Prescaler = 6;
  hcan.Init.Mode = CAN_MODE_NORMAL;
  hcan.Init.SyncJumpWidth = CAN_SJW_1TQ;
  hcan.Init.TimeSeg1 = CAN_BS1_13TQ;
  hcan.Init.TimeSeg2 = CAN_BS2_2TQ;
  hcan.Init.TimeTriggeredMode = DISABLE;
  hcan.Init.AutoBusOff = DISABLE;
  hcan.Init.AutoWakeUp = DISABLE;
  hcan.Init.AutoRetransmission = DISABLE;
  hcan.Init.ReceiveFifoLocked = DISABLE;
  hcan.Init.TransmitFifoPriority = DISABLE;
  if (HAL_CAN_Init(&hcan) != HAL_OK)
  {
    Error_Handler();
  }
  /* USER CODE BEGIN CAN_Init 2 */
  CAN_FilterTypeDef canfilterconfig;

       canfilterconfig.FilterActivation = CAN_FILTER_ENABLE;
       canfilterconfig.FilterBank = 13;
       canfilterconfig.FilterFIFOAssignment = CAN_FILTER_FIFO0;
       canfilterconfig.FilterIdHigh = 0x0000;
       canfilterconfig.FilterIdLow = 0;
       canfilterconfig.FilterMaskIdHigh = 0x0000;
       canfilterconfig.FilterMaskIdLow = 0x0000;
       canfilterconfig.FilterMode = CAN_FILTERMODE_IDMASK;
       canfilterconfig.FilterScale = CAN_FILTERSCALE_32BIT;


       HAL_CAN_ConfigFilter(&hcan, &canfilterconfig);
  /* USER CODE END CAN_Init 2 */

}

/**
  * @brief TIM17 Initialization Function
  * @param None
  * @retval None
  */
static void MX_TIM17_Init(void)
{

  /* USER CODE BEGIN TIM17_Init 0 */

  /* USER CODE END TIM17_Init 0 */

  /* USER CODE BEGIN TIM17_Init 1 */

  /* USER CODE END TIM17_Init 1 */
  htim17.Instance = TIM17;
  htim17.Init.Prescaler = 2-1;
  htim17.Init.CounterMode = TIM_COUNTERMODE_UP;
  htim17.Init.Period = 53-1;
  htim17.Init.ClockDivision = TIM_CLOCKDIVISION_DIV1;
  htim17.Init.RepetitionCounter = 0;
  htim17.Init.AutoReloadPreload = TIM_AUTORELOAD_PRELOAD_DISABLE;
  if (HAL_TIM_Base_Init(&htim17) != HAL_OK)
  {
    Error_Handler();
  }
  /* USER CODE BEGIN TIM17_Init 2 */

  /* USER CODE END TIM17_Init 2 */

}

/**
  * @brief USART1 Initialization Function
  * @param None
  * @retval None
  */
static void MX_USART1_UART_Init(void)
{

  /* USER CODE BEGIN USART1_Init 0 */

  /* USER CODE END USART1_Init 0 */

  /* USER CODE BEGIN USART1_Init 1 */

  /* USER CODE END USART1_Init 1 */
  huart1.Instance = USART1;
  huart1.Init.BaudRate = 921600;
  huart1.Init.WordLength = UART_WORDLENGTH_8B;
  huart1.Init.StopBits = UART_STOPBITS_1;
  huart1.Init.Parity = UART_PARITY_NONE;
  huart1.Init.Mode = UART_MODE_TX_RX;
  huart1.Init.HwFlowCtl = UART_HWCONTROL_NONE;
  huart1.Init.OverSampling = UART_OVERSAMPLING_16;
  huart1.Init.OneBitSampling = UART_ONE_BIT_SAMPLE_DISABLE;
  huart1.AdvancedInit.AdvFeatureInit = UART_ADVFEATURE_NO_INIT;
  if (HAL_UART_Init(&huart1) != HAL_OK)
  {
    Error_Handler();
  }
  /* USER CODE BEGIN USART1_Init 2 */

  /* USER CODE END USART1_Init 2 */

}

/**
  * @brief GPIO Initialization Function
  * @param None
  * @retval None
  */
static void MX_GPIO_Init(void)
{
  GPIO_InitTypeDef GPIO_InitStruct = {0};

  /* GPIO Ports Clock Enable */
  __HAL_RCC_GPIOB_CLK_ENABLE();
  __HAL_RCC_GPIOA_CLK_ENABLE();

  /*Configure GPIO pin Output Level */
  HAL_GPIO_WritePin(GPIOB, GPIO_PIN_1|GPIO_PIN_10|GPIO_PIN_11, GPIO_PIN_RESET);

  /*Configure GPIO pins : PB1 PB10 PB11 */
  GPIO_InitStruct.Pin = GPIO_PIN_1|GPIO_PIN_10|GPIO_PIN_11;
  GPIO_InitStruct.Mode = GPIO_MODE_OUTPUT_PP;
  GPIO_InitStruct.Pull = GPIO_NOPULL;
  GPIO_InitStruct.Speed = GPIO_SPEED_FREQ_LOW;
  HAL_GPIO_Init(GPIOB, &GPIO_InitStruct);

}

/* USER CODE BEGIN 4 */

/* USER CODE END 4 */

/**
  * @brief  This function is executed in case of error occurrence.
  * @retval None
  */
void Error_Handler(void)
{
  /* USER CODE BEGIN Error_Handler_Debug */
  /* User can add his own implementation to report the HAL error return state */
  __disable_irq();
  while (1)
  {
	  HAL_GPIO_WritePin(GPIOB, GPIO_PIN_1|GPIO_PIN_10|GPIO_PIN_11, GPIO_PIN_RESET);
	  HAL_Delay(100);
	  HAL_GPIO_WritePin(GPIOB, GPIO_PIN_1|GPIO_PIN_10|GPIO_PIN_11, GPIO_PIN_SET);
  }
  /* USER CODE END Error_Handler_Debug */
}

#ifdef  USE_FULL_ASSERT
/**
  * @brief  Reports the name of the source file and the source line number
  *         where the assert_param error has occurred.
  * @param  file: pointer to the source file name
  * @param  line: assert_param error line source number
  * @retval None
  */
void assert_failed(uint8_t *file, uint32_t line)
{
  /* USER CODE BEGIN 6 */
  /* User can add his own implementation to report the file name and line number,
     ex: printf("Wrong parameters value: file %s on line %d\r\n", file, line) */
  /* USER CODE END 6 */
}
#endif /* USE_FULL_ASSERT */
