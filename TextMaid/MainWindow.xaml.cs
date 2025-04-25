using System;
using System.Linq;
using System.Text.RegularExpressions; // 이모지 제거를 위해 추가
using System.Windows;
using System.Windows.Media; // Brushes 사용을 위해 추가
using TextMaid.Language; // 다국어 리소스 접근용 네임스페이스

namespace TextMaid
{
    public partial class MainWindow : Window
    {
        // 감지 및 제거할 유니코드 문자 정의
        private const char NarrowNoBreakSpace = '\u202F';
        private const char WordJoiner = '\u2060';
        private const char ZeroWidthSpace = '\u200B';
        private const char NoBreakSpace = '\u00A0';
        private readonly MaidFace maidFace = new MaidFace();


        // 감지할 공백 유니코드 문자 배열
        private readonly char[] _whitespaceCharsToCheck = {
            NarrowNoBreakSpace,
            WordJoiner,
            ZeroWidthSpace,
            NoBreakSpace
        };

        public MainWindow()
        {
            InitializeComponent();
            // 다국어 타이틀 설정
            this.Title = Resource.WindowTitle;

            UpdateMaidFace(MaidExpression.Default);

        }

        // 메이드 표정을 업데이트하는 메서드
        private void UpdateMaidFace(MaidExpression expression)
        {
            // 클래스 인스턴스 변수명 변경: maidFace
            var faceBitmap = maidFace.GetFaceBitmap(expression);
            emojiOverlay.Source = faceBitmap;
        }


        // 1. Check 버튼 클릭 이벤트 핸들러
        private void OnCheckClicked(object sender, RoutedEventArgs e)
        {
            string inputText = inputTextBox.Text;
            bool found = _whitespaceCharsToCheck.Any(c => inputText.Contains(c));

            if (found)
            {
                // 유니코드 감지됨 메시지 출력
                SetResultMessage(Resource.UnicodeDetected, Brushes.Red);
                UpdateMaidFace(MaidExpression.Weird);

            }
            else
            {
                // 유니코드 없음 메시지 출력
                SetResultMessage(Resource.UnicodeNotDetected, Brushes.SkyBlue);
                UpdateMaidFace(MaidExpression.Default);
            }
        }

        // 2. Clean 버튼 클릭 이벤트 핸들러
        private void OnCleanClicked(object sender, RoutedEventArgs e)
        {
            string inputText = inputTextBox.Text;
            string cleanedText = inputText;

            // 정의된 공백 유니코드 문자를 빈 문자열로 치환
            foreach (char c in _whitespaceCharsToCheck)
            {
                cleanedText = cleanedText.Replace(c.ToString(), string.Empty);
            }

            inputTextBox.Text = cleanedText;

            // 유니코드 클린 완료 메시지 출력
            SetResultMessage(Resource.UnicodeCleaned, Brushes.LimeGreen);
            UpdateMaidFace(MaidExpression.Occasional);

        }

        // 3. Emoji Clean 버튼 클릭 이벤트 핸들러
        private void OnEmojiCleanClicked(object sender, RoutedEventArgs e)
        {
            string inputText = inputTextBox.Text;
            string cleanedText = inputText;

            // 공백 유니코드 제거
            foreach (char c in _whitespaceCharsToCheck)
            {
                cleanedText = cleanedText.Replace(c.ToString(), string.Empty);
            }

            // 이모지 제거 (정규 표현식 수정)
            // \p{So}: 기호 및 그림 문자
            // \p{Cs}: 대리쌍
            // \uFE0F: Variation Selector-16 (이모지 스타일 지정자) 추가
            try
            {
                // 정규식에 \uFE0F 추가
                cleanedText = Regex.Replace(cleanedText, @"\p{So}|\p{Cs}|\uFE0F", string.Empty, RegexOptions.Compiled);
            }
            catch (RegexMatchTimeoutException ex)
            {
                SetResultMessage($"이모지 처리 중 오류 발생 (시간 초과): {ex.Message}", Brushes.OrangeRed);
                return;
            }
            // 만약 다른 Variation Selector(U+FE00~U+FE0E)도 제거하고 싶다면 범위를 지정할 수 있음
            // cleanedText = Regex.Replace(cleanedText, @"\p{So}|\p{Cs}|[\uFE00-\uFE0F]", string.Empty, RegexOptions.Compiled);
            // 하지만 보통 이모지에는 U+FE0F만 주로 사용됨

            inputTextBox.Text = cleanedText;

            // 유니코드 + 이모지 클린 완료 메시지 출력
            SetResultMessage(Resource.EmojiCleaned, Brushes.LimeGreen);
            UpdateMaidFace(MaidExpression.Smile);
        }

        // 4. Save 버튼 클릭 이벤트 핸들러
        private void OnSaveClicked(object sender, RoutedEventArgs e)
        {
            try
            {
                // 텍스트 클립보드 복사
                Clipboard.SetText(inputTextBox.Text);
                SetResultMessage(Resource.ClipboardCopied, Brushes.SkyBlue);
                UpdateMaidFace(MaidExpression.OccasionalSuccess);

            }
            catch (Exception ex)
            {
                // 클립보드 복사 실패 시 메시지 출력
                SetResultMessage(string.Format(Resource.ClipboardError, ex.Message), Brushes.OrangeRed);
                UpdateMaidFace(MaidExpression.Failure1);
            }
        }

        // 결과 메시지 및 색상 설정 도우미 함수
        private void SetResultMessage(string message, Brush foregroundColor)
        {
            resultTextBox.Foreground = foregroundColor;
            resultTextBox.Text = $"[{DateTime.Now:HH:mm:ss}] {message}"; // 타임스탬프 추가
        }
    }
}