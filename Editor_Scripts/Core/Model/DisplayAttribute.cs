using System;

namespace SpaceAR.Core.Model
{
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Method | AttributeTargets.Parameter | AttributeTargets.Property, AllowMultiple = false)]
    public sealed class DisplayAttribute : Attribute
    {
        private string _shortName = default;

        private string _name = default;

        private string _description = default;

        //
        // 요약:
        //     표 형태 창의 열 레이블에 사용되는 값을 가져오거나 설정합니다.
        //
        // 반환 값:
        //     표 형태 창의 열 레이블에 사용되는 값입니다.
        public string ShortName
        {
            
            get
            {
                return _shortName;
            }
            
            set
            {
                if (_shortName != value)
                {
                    _shortName = value;
                }
            }
        }

        //
        // 요약:
        //     UI에 표시하는 데 사용되는 값을 가져오거나 설정합니다.
        //
        // 반환 값:
        //     UI에 표시하는 데 사용되는 값입니다.
        
        public string Name
        {
            
            get
            {
                return _name;
            }
            
            set
            {
                if (_name != value)
                {
                    _name = value;
                }
            }
        }

        //
        // 요약:
        //     UI에 설명을 표시하는 데 사용되는 값을 가져오거나 설정합니다.
        //
        // 반환 값:
        //     UI에 설명을 표시하는 데 사용되는 값입니다.
        
        public string Description
        {
            
            get
            {
                return _description;
            }
            
            set
            {
                if (_description != value)
                {
                    _description = value;
                }
            }
        }

        //
        // 요약:
        //     System.ComponentModel.DataAnnotations.DisplayAttribute 클래스의 새 인스턴스를 초기화합니다.
        
        public DisplayAttribute()
        {
        }

        //
        // 요약:
        //     System.ComponentModel.DataAnnotations.DisplayAttribute.ShortName 속성의 값을 반환합니다.
        //
        // 반환 값:
        //     System.ComponentModel.DataAnnotations.DisplayAttribute.ShortName 속성이 지정된 경우와
        //     System.ComponentModel.DataAnnotations.DisplayAttribute.ResourceType 속성이 리소스 키를
        //     나타내는 경우 System.ComponentModel.DataAnnotations.DisplayAttribute.ShortName 속성의
        //     지역화된 문자열이고, 그렇지 않으면 System.ComponentModel.DataAnnotations.DisplayAttribute.ShortName
        //     속성의 지역화되지 않은 값입니다.
        
        public string GetShortName()
        {
            return _shortName ?? GetName();
        }

        //
        // 요약:
        //     UI의 필드 표시에 사용되는 값을 반환합니다.
        //
        // 반환 값:
        //     System.ComponentModel.DataAnnotations.DisplayAttribute.Name 속성이 지정되었으며 System.ComponentModel.DataAnnotations.DisplayAttribute.ResourceType
        //     속성이 리소스 키를 나타내면 System.ComponentModel.DataAnnotations.DisplayAttribute.Name 속성의
        //     지역화된 문자열이고, 그렇지 않으면 System.ComponentModel.DataAnnotations.DisplayAttribute.Name
        //     속성의 지역화되지 않은 값입니다.
        //
        // 예외:
        //   T:System.InvalidOperationException:
        //     System.ComponentModel.DataAnnotations.DisplayAttribute.ResourceType 속성 및 System.ComponentModel.DataAnnotations.DisplayAttribute.Name
        //     속성이 초기화되지만 System.ComponentModel.DataAnnotations.DisplayAttribute.Name 속성에 대한
        //     System.ComponentModel.DataAnnotations.DisplayAttribute.ResourceType 값과 일치하는 이름을
        //     가진 공용 정적 속성을 찾을 수 없습니다.
        
        public string GetName()
        {
            return _name;
        }

        //
        // 요약:
        //     System.ComponentModel.DataAnnotations.DisplayAttribute.Description 속성의 값을 반환합니다.
        //
        // 반환 값:
        //     System.ComponentModel.DataAnnotations.DisplayAttribute.ResourceType이 지정되었으며 System.ComponentModel.DataAnnotations.DisplayAttribute.Description
        //     속성이 리소스 키를 나타내면 지역화된 설명이고, 그렇지 않으면 System.ComponentModel.DataAnnotations.DisplayAttribute.Description
        //     속성의 지역화되지 않은 값입니다.
        //
        // 예외:
        //   T:System.InvalidOperationException:
        //     System.ComponentModel.DataAnnotations.DisplayAttribute.ResourceType 속성 및 System.ComponentModel.DataAnnotations.DisplayAttribute.Description
        //     속성이 초기화되지만 System.ComponentModel.DataAnnotations.DisplayAttribute.Description
        //     속성에 대한 System.ComponentModel.DataAnnotations.DisplayAttribute.ResourceType 값과
        //     일치하는 이름을 가진 공용 정적 속성을 찾을 수 없습니다.
        
        public string GetDescription()
        {
            return _description;
        }
    }
}
