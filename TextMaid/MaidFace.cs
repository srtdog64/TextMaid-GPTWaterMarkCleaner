using System;
using System.Collections.Generic; // Dictionary 사용
using System.Windows;
using System.Windows.Media.Imaging;

namespace TextMaid
{
    public enum MaidExpression
    {
        Default,          // 1번 -> (0, 0) - 기본
        Smile,            // 3번 -> (0, 2) - 웃는 얼굴
        Annoyed,          // 5번 -> (1, 1) - 짜증
        Failure1,         // 6번 -> (1, 2) - 실패1
        Occasional,       // 7번 -> (2, 0) - 가끔 나오는 얼굴
        OccasionalSuccess,// 8번 -> (2, 1) - 성공시 가끔
        Failure2,         // 9번 -> (2, 2) - 실패2
        Weird,            // 12번 -> (3, 2) - 임의 표정1
        Tired,            // 14번 -> (4, 0) - 임의 표정2
        Happy             // 15번 -> (4, 1) - 임의 표정3
    }

    // 클래스 이름을 MaidFace로 변경
    public class MaidFace
    {
        // --- 설정 값 --- 
        private const int OffsetX = 24; 
        private const int OffsetY = 30; 
        private const int SpriteWidth = 150; 
        private const int SpriteHeight = 86; 
        // -----------------------------------------------------------

        private readonly Uri spriteSheetUri = new Uri("pack://application:,,,/Image/maid_face.png"); // 경로 확인!

        private readonly Dictionary<MaidExpression, (int row, int col)> expressionCoordinates =
            new Dictionary<MaidExpression, (int row, int col)>
            {
                { MaidExpression.Default, (0, 0) },
                { MaidExpression.Smile, (0, 2) },
                { MaidExpression.Annoyed, (1, 1) },
                { MaidExpression.Failure1, (1, 2) },
                { MaidExpression.Occasional, (2, 0) },
                { MaidExpression.OccasionalSuccess, (2, 1) },
                { MaidExpression.Failure2, (2, 2) },
                { MaidExpression.Weird, (3, 2) },
                { MaidExpression.Tired, (4, 0) },
                { MaidExpression.Happy, (4, 1) }
            };

        private BitmapImage cachedSpriteSheet = null;

        public MaidFace()
        {
            LoadSpriteSheet();
        }

        // 원본 이미지 로드 (오류 처리 포함)
        private void LoadSpriteSheet()
        {
            // 변수명 변경: cachedSpriteSheet
            if (cachedSpriteSheet == null)
            {
                try
                {
                    // 변수명 변경: spriteSheetUri
                    cachedSpriteSheet = new BitmapImage(spriteSheetUri);
                    Console.WriteLine($"Sprite sheet loaded: {cachedSpriteSheet.PixelWidth}x{cachedSpriteSheet.PixelHeight}");
                    cachedSpriteSheet.Freeze();
                }
                catch (Exception ex)
                {
                    // 변수명 변경: spriteSheetUri
                    Console.WriteLine($"스프라이트 시트 로드 오류 ({spriteSheetUri}): {ex.Message}");
                    cachedSpriteSheet = null;
                }
            }
        }

        // 오프셋 포함 소스 사각형 계산
        private Int32Rect CalculateSourceRect(int row, int col)
        {
            int calculatedX = OffsetX + (col * SpriteWidth);
            int calculatedY = OffsetY + (row * SpriteHeight);
            return new Int32Rect(calculatedX, calculatedY, SpriteWidth, SpriteHeight);
        }

        // 특정 표정에 대한 CroppedBitmap 반환
        public CroppedBitmap GetFaceBitmap(MaidExpression expression)
        {
            // 변수명 변경: cachedSpriteSheet
            if (cachedSpriteSheet == null)
            {
                LoadSpriteSheet();
                if (cachedSpriteSheet == null) return null;
            }

            // 변수명 변경: expressionCoordinates
            if (expressionCoordinates.TryGetValue(expression, out var coords))
            {
                try
                {
                    var sourceRect = CalculateSourceRect(coords.row, coords.col);

                    // 변수명 변경: cachedSpriteSheet
                    if (sourceRect.X >= 0 && sourceRect.Y >= 0 &&
                        sourceRect.X + sourceRect.Width <= cachedSpriteSheet.PixelWidth &&
                        sourceRect.Y + sourceRect.Height <= cachedSpriteSheet.PixelHeight)
                    {
                        // 변수명 변경: cachedSpriteSheet
                        var cropped = new CroppedBitmap(cachedSpriteSheet, sourceRect);
                        cropped.Freeze();
                        return cropped;
                    }
                    else
                    {
                        // 변수명 변경: cachedSpriteSheet
                        Console.WriteLine($"오류: 계산된 소스 사각형({sourceRect})이 비트맵 범위({cachedSpriteSheet.PixelWidth}x{cachedSpriteSheet.PixelHeight})를 벗어남. 표정: {expression}. OffsetX/Y 및 Sprite Width/Height 값을 확인하세요.");
                        return GetDefaultFaceBitmap();
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"이미지 자르기 오류 ({expression}): {ex.Message}");
                    return GetDefaultFaceBitmap();
                }
            }
            else
            {
                Console.WriteLine($"정의되지 않은 표정 요청: {expression}");
                return GetDefaultFaceBitmap();
            }
        }

        // 기본 표정 비트맵 가져오기
        private CroppedBitmap GetDefaultFaceBitmap()
        {
            // 변수명 변경: cachedSpriteSheet, expressionCoordinates
            if (cachedSpriteSheet != null && expressionCoordinates.TryGetValue(MaidExpression.Default, out var defaultCoords))
            {
                try
                {
                    var sourceRect = CalculateSourceRect(defaultCoords.row, defaultCoords.col);
                    // 변수명 변경: cachedSpriteSheet
                    if (sourceRect.X >= 0 && sourceRect.Y >= 0 &&
                       sourceRect.X + sourceRect.Width <= cachedSpriteSheet.PixelWidth &&
                       sourceRect.Y + sourceRect.Height <= cachedSpriteSheet.PixelHeight)
                    {
                        // 변수명 변경: cachedSpriteSheet
                        var cropped = new CroppedBitmap(cachedSpriteSheet, sourceRect);
                        cropped.Freeze();
                        return cropped;
                    }
                }
                catch { }
            }
            return null;
        }
    }
}