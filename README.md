# 🧛 Vampire Survival-like (Unity Portfolio Project)

**Vampire Survivors 스타일의 2D 생존 액션 게임 프로토타입**  
Unity 엔진 기반으로 **확장성과 유지보수성을 고려한 아키텍처 설계**를 목표로 개발한 포트폴리오 프로젝트입니다.

> 🎮 실제 플레이 영상: `vampiresurvival_like.mp4` 포함

---

## 🧩 프로젝트 개요

| 항목 | 내용 |
|-----|-----|
| 장르 | 2D Survival / Action |
| 엔진 | Unity |
| 플랫폼 | PC |
| 목적 | 게임 구조 설계 능력 및 코드 품질 중심 포트폴리오 |
| 핵심 키워드 | Entity 기반 구조, 역할 분리, 확장 가능한 설계 |

---

## 🏗️ 아키텍처 설계

본 프로젝트는 **기능 확장과 유지보수에 강한 구조**를 목표로 다음과 같은 레이어 구조로 설계되었습니다.

Script
├─ Entity → 게임 월드의 모든 객체 논리 계층
│ ├─ Character → 플레이어, 몬스터 등 캐릭터 로직
│ ├─ Dummy → 테스트용 엔티티
│ ├─ EventTrigger → 이벤트 트리거 엔티티
│ └─ ...
├─ View → 화면 표현 및 UI
│ ├─ UI
│ └─ ...

markdown
코드 복사

### 🧠 핵심 설계 원칙

- **Entity 중심 설계**
  - 모든 게임 오브젝트의 로직을 `Entity` 계층에서 관리
- **View와 Logic 분리**
  - 게임 로직과 화면/UI 완전 분리
- **확장 가능한 캐릭터 시스템**
  - `EnemyCharacterEntity` 파생 구조로 다양한 몬스터 구현
- **역할 기반 코드 구조**
  - Character / Event / UI / System 별 명확한 책임 분리

---

## 👾 구현 요소

### ✔ 플레이어 시스템
- 이동
- 공격
- 상태 관리

### ✔ 적 캐릭터 시스템
- `SlimeKingEnemyCharacterEntity`
- `SuicideBombingEnemyCharacterEntity`
- `ShielderEnemyCharacterEntity`
- 각 적마다 **개별 AI 및 행동 로직 구현**

### ✔ 전투 & 이벤트
- 충돌 판정
- 체력 시스템
- EventTrigger 기반 이벤트 처리

### ✔ UI 시스템
- `UiViewInitializer` 기반 UI 초기화 구조
- 게임 상태에 따른 UI 제어

---

## 🧪 기술적 특징

- **상속 기반 캐릭터 구조**
- **데이터 중심 설계(Entity 중심)**
- **의존성 최소화**
- **확장 시 기존 코드 수정 최소화**

---

## 📽️ 플레이 영상

프로젝트 루트에 포함된 영상 참고:

vampiresurvival_like.mp4

yaml
코드 복사

---

## 💡 개발 의도

> 단순한 기능 구현이 아니라  
> **실제 서비스 환경에서 유지보수 가능한 게임 구조**를 목표로 설계했습니다.

특히 캐릭터, 이벤트, UI, 시스템을 분리하여  
추후 신규 캐릭터, 스킬, 시스템 추가 시  
**기존 코드 수정 없이 확장 가능**하도록 구조를 설계했습니다.

---

## 🧑‍💻 개발자

**이재우**  
Unity 게임 클라이언트 개발자 (10년+)  
AI · Backend · Network · Mobile 개발 경험
